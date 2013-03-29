using NUnit.Framework;
using Tasko.Server.Models;

namespace Tasko.Tests
{
    public class TaskTests
    {
        private Task task;

        [SetUp]
        public void Setup()
        {
            this.task = new Task("foo", "cat1");
        }

        [TestFixture]
        public class Constructor : TaskTests
        {
            [Test]
            public void DescriptionWasSet()
            {
                Assert.That(task.Description == "foo");
            }

            [Test]
            public void CategoryWasAdded()
            {
                Assert.That(task.Categories.Count == 1);
                Assert.That(task.Categories[0] == "cat1");
            }
        }
    }
}
