using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NUnit.Framework;
using Raven.Client.Embedded;
using Tasko.Server.Controllers;
using Tasko.Server.Models;

namespace Tasko.Tests.Integration
{
    public class TasksControllerTests
    {
        [SetUp]
        public void Setup()
        {
            RavenController.Store = new EmbeddableDocumentStore { RunInMemory = true };
            RavenController.Store.Initialize();
        }

        /// <summary>
        /// creates a controller with an open session
        /// </summary>
        public TasksController GetController()
        {
            var c = new TasksController();
            Helper.SetupControllerForTests(c);

            c.RavenSession = RavenController.Store.OpenSession();
            return c;
        }

        /// <summary>
        /// creates a TaskCreateDto
        /// </summary>
        public TaskCreateDto GetDto(string description, string category)
        {
            var dto = new TaskCreateDto();
            dto.Description = description;
            dto.Category = category;
            return dto;
        }

        /// <summary>
        /// returns the Task object from the "create task" response
        /// </summary>
        public Task GetTaskFromResponse(HttpResponseMessage resp)
        {
            return resp.Content.ReadAsAsync<Task>().Result;
        }

        [TestFixture]
        public class Post : TasksControllerTests
        {
            [Test]
            public void InsertsOneTask()
            {
                var dto = GetDto("foo", "bar");

                using (var c = GetController())
                {
                    c.Post(dto);
                }

                using (var c = GetController())
                {
                    var tasks = c.Get();
                    Assert.That(tasks.Count() == 1);

                    var task = tasks.First();
                    
                    Assert.AreEqual("foo", task.Description);
                    Assert.AreEqual("bar", task.Categories.First());
                }
            }

            [Test]
            public void ReturnsTaskWithId()
            {
                var dto = GetDto("foo", "bar");

                using (var c = GetController())
                {
                    var resp = c.Post(dto);
                    var task = GetTaskFromResponse(resp);

                    Assert.Greater(task.Id, 0);
                    Assert.AreEqual("foo", task.Description);
                    Assert.AreEqual("bar", task.Categories.First());
                }
            }

            [Test]
            public void ReturnsCreatedOnSuccess()
            {
                using (var c = GetController())
                {
                    var dto = GetDto("foo", "bar");
                    var resp = c.Post(dto);

                    Assert.AreEqual(HttpStatusCode.Created, resp.StatusCode);
                }
            }

            [Test]
            public void ReturnsBadRequestWhenNoDtoIsPassed()
            {
                using (var c = GetController())
                {
                    var resp = c.Post(null);
                    Assert.AreEqual(HttpStatusCode.BadRequest, resp.StatusCode);
                }
            }
        }

        [TestFixture]
        public class Get : TasksControllerTests
        {
            [Test]
            public void LoadsOneTaskById()
            {
                int taskId = 0;

                using (var c = GetController())
                {
                    var dto = GetDto("foo", "bar");
                    var resp = c.Post(dto);
                    var task = GetTaskFromResponse(resp);
                    taskId = task.Id;
                    Assert.AreNotEqual(0, taskId);
                }

                using (var c = GetController())
                {
                    var task = c.Get(taskId);

                    Assert.IsNotNull(task);
                    Assert.AreEqual("foo", task.Description);
                    Assert.AreEqual("bar", task.Categories.First());
                }
            }

            [Test]
            public void ReturnsNotFoundWhenLoadingNonExistingTaskById()
            {
                using (var c = GetController())
                {
                    var ex = Assert.Throws<HttpResponseException>(() => c.Get(100));
                    Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
                }
            }

            [Test]
            public void LoadsAllTasks()
            {
                using (var c = GetController())
                {
                    var dto = GetDto("foo1", "bar");
                    c.Post(dto);
                    dto = GetDto("foo2", "bar");
                    c.Post(dto);
                    dto = GetDto("foo3", "bar");
                    c.Post(dto);
                }

                using (var c = GetController())
                {
                    var tasks = c.Get();
                    Assert.IsNotNull(tasks);
                    Assert.AreEqual(3, tasks.Count());
                }
            }

            [Test]
            public void LoadTasksByCategory()
            {
                using (var c = GetController())
                {
                    var dto = GetDto("foo1", "cat1");
                    c.Post(dto);
                    dto = GetDto("foo2", "cat1");
                    c.Post(dto);
                    dto = GetDto("foo3", "cat2");
                    c.Post(dto);
                }

                using (var c = GetController())
                {
                    var tasks = c.Get("cat1");
                    Assert.IsNotNull(tasks);
                    Assert.AreEqual(2, tasks.Count());
                }
            }
        }

        [TestFixture]
        public class FinishAndReopenTask : TasksControllerTests
        {
            [Test]
            public void TaskIsFinishedAndReopened()
            {
                int taskid = 0;

                using (var c = GetController())
                {
                    var dto = GetDto("foo", "bar");
                    var resp = c.Post(dto);
                    var task = GetTaskFromResponse(resp);
                    taskid = task.Id;
                    Assert.AreNotEqual(0, taskid);
                }

                using (var c = GetController())
                {
                    c.FinishTask(taskid);
                }

                // task should be finished now
                using (var c = GetController())
                {
                    var task = c.Get(taskid);
                    Assert.True(task.IsFinished);

                    c.ReopenTask(taskid);
                }

                // task shouldn't be finished now
                using (var c = GetController())
                {
                    var task = c.Get(taskid);
                    Assert.False(task.IsFinished);
                }
            }
        }

        [TestFixture]
        public class ChangeDescription : TasksControllerTests
        {
            [Test]
            public void DescriptionIsChanged()
            {
                int taskid;

                using (var c = GetController())
                {
                    var resp = c.Post(GetDto("foo", "bar"));
                    var task = GetTaskFromResponse(resp);
                    taskid = task.Id;

                    c.ChangeDescription(taskid, new ChangeDescriptionDto { Description = "test" });
                }

                using (var c = GetController())
                {
                    var task = c.Get(taskid);
                    Assert.AreEqual("test", task.Description);
                }

            }

            [Test]
            public void ReturnsBadRequestWhenDtoIsNull()
            {
                using (var c = GetController())
                {
                    var resp = c.Post(GetDto("foo", "bar"));
                    var task = GetTaskFromResponse(resp);

                    var resp2 = c.ChangeDescription(task.Id, null);
                    Assert.AreEqual(HttpStatusCode.BadRequest, resp2.StatusCode);
                }
            }
        }

        [TestFixture]
        public class GetCategories : TasksControllerTests
        {
            [Test]
            public void ReturnsOneCategory()
            {
                using (var c = GetController())
                {
                    var resp = c.Post(GetDto("foo", "bar"));
                    var task = GetTaskFromResponse(resp);

                    var categories = c.GetCategories(task.Id);
                    Assert.AreEqual(1, categories.Count());
                    Assert.Contains("bar",categories.ToList());
                }
            }
        }
    }
}
