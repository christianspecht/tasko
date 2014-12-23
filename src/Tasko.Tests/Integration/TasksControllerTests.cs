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
                    Assert.That(task.Description == "foo");
                    Assert.That(task.Categories.First() == "bar");
                }
            }

            [Test]
            public void ReturnsTaskWithId()
            {
                var dto = GetDto("foo", "bar");

                using (var c = GetController())
                {
                    var resp = c.Post(dto);
                    var task = resp.Content.ReadAsAsync<Task>().Result;

                    Assert.That(task.Id > 0);
                    Assert.That(task.Description == "foo");
                    Assert.That(task.Categories.First() == "bar");
                }
            }

            [Test]
            public void ReturnsCreatedOnSuccess()
            {
                using (var c = GetController())
                {
                    var dto = GetDto("foo", "bar");
                    var resp = c.Post(dto);

                    Assert.That(resp.StatusCode == HttpStatusCode.Created);
                }
            }

            [Test]
            public void ReturnsBadRequestWhenNoDtoIsPassed()
            {
                using (var c = GetController())
                {
                    var resp = c.Post(null);
                    Assert.That(resp.StatusCode == HttpStatusCode.BadRequest);
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
                    var task = resp.Content.ReadAsAsync<Task>().Result;
                    taskId = task.Id;
                    Assert.That(taskId != 0);
                }

                using (var c = GetController())
                {
                    var task = c.Get(taskId);

                    Assert.That(task != null);
                    Assert.That(task.Description == "foo");
                    Assert.That(task.Categories.First() == "bar");
                }
            }

            [Test]
            public void ReturnsNotFoundWhenLoadingNonExistingTaskById()
            {
                using (var c = GetController())
                {
                    var ex = Assert.Throws<HttpResponseException>(() => c.Get(100));
                    Assert.That(ex.Response.StatusCode == HttpStatusCode.NotFound);
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
                    Assert.That(tasks != null);
                    Assert.That(tasks.Count() == 3);
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
                    Assert.That(tasks != null);
                    Assert.That(tasks.Count() == 2);
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
                    var task = resp.Content.ReadAsAsync<Task>().Result;
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
    }
}
