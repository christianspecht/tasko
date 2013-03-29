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
            public void ThrowsWhenDescriptionIsEmpty()
            {
                Assert.Catch(() => new Task("", "cat1"));
            }

            [Test]
            public void CategoryWasAdded()
            {
                Assert.That(task.Categories.Count == 1);
                Assert.That(task.Categories[0] == "cat1");
            }

            [Test]
            public void ThrowsWhenCategoryIsEmpty()
            {
                Assert.Catch(() => new Task("foo", ""));
            }
        }

        [TestFixture]
        public class Description : TaskTests
        {
            [Test]
            public void ThrowsWhenEmpty()
            {
                Assert.Catch(() => task.Description = null);
                Assert.Catch(() => task.Description = "");
                Assert.Catch(() => task.Description = "   ");
            }
        }

        [TestFixture]
        public class AddCategory : TaskTests
        {
            [Test]
            public void CategoryWasAdded()
            {
                int numberOfCategories = task.Categories.Count;
                task.AddCategory("cat2");

                Assert.That(task.Categories.Count == numberOfCategories + 1);
                Assert.That(task.Categories.Contains("cat2"));
            }

            [Test]
            public void ThrowsWhenCategoryIsEmpty()
            {
                Assert.Catch(() => task.AddCategory(null));
                Assert.Catch(() => task.AddCategory(""));
                Assert.Catch(() => task.AddCategory("   "));
            }
        }
    }
}
