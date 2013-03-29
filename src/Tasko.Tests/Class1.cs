using System.Collections.Generic;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;
using Tasko.Server.Models;

namespace Tasko.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void DummyTest()
        {
            // setup
            IDocumentStore store = new EmbeddableDocumentStore { RunInMemory = true };
            store.Initialize();

            int id;

            // save
            using (var session = store.OpenSession())
            {
                var task = new Task("test", "cat1");
                task.AddCategory("cat2");

                session.Store(task);
                id = task.Id;

                session.SaveChanges();
            }

            // load
            using (var session = store.OpenSession())
            {
                var task = session.Load<Task>(id);

                Assert.That(task.Id == id);
                Assert.That(task.Description == "test");
                Assert.That(task.Categories.Count == 2);
            }
        }
    }
}
