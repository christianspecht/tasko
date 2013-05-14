using System.Linq;
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
            c.RavenSession = RavenController.Store.OpenSession();
            return c;
        }

        /// <summary>
        /// creates a TaskCreateDto
        /// </summary>
        public TaskCreateDto GetDto(string description, string category)
        {
            var dto = new TaskCreateDto();
            dto.Description = "foo";
            dto.Category = "bar";
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
        }
    }
}
