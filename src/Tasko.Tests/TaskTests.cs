using System;
using System.Threading;
using NUnit.Framework;
using Tasko.Server.Models;

namespace Tasko.Tests
{
    public class TaskTests
    {
        private Task task;
        private DateTime lastEditedAt;

        [SetUp]
        public void Setup()
        {
            this.task = new Task("foo", "cat1");

            // save value for tests (to be able to tell if the value was changed after creation)
            // and pause to make sure that the value is actually different after being set a second time
            this.lastEditedAt = this.task.LastEditedAt;
            Thread.Sleep(3);
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

            [Test]
            public void CreatedAtWasSet()
            {
                Assert.That(task.CreatedAt != DateTime.MinValue);
            }

            [Test]
            public void LastEditedWasSet()
            {
                Assert.That(task.LastEditedAt != DateTime.MinValue);
            }

            [Test]
            public void TaskIsNotFinished()
            {
                Assert.IsFalse(task.IsFinished);
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

            [Test]
            public void LastEditedChangesWhenDescriptionIsChanged()
            {
                task.Description = "bar";
                Assert.That(task.LastEditedAt != this.lastEditedAt);
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

            [Test]
            public void LastEditedChangesWhenCategoryIsAdded()
            {
                task.AddCategory("cat2");
                Assert.That(task.LastEditedAt != this.lastEditedAt);
            }
        }

        [TestFixture]
        public class Finish : TaskTests
        {
            [Test]
            public void TaskIsFinished()
            {
                task.Finish();
                Assert.That(task.IsFinished);
            }

            [Test]
            public void FinishedAtIsSet()
            {
                task.Finish();
                Assert.That(task.FinishedAt.HasValue);
            }

            [Test]
            public void LastEditedIsSet()
            {
                task.Finish();
                Assert.That(task.LastEditedAt != this.lastEditedAt);
            }

            [Test]
            public void ThrowsWhenTaskIsFinishedTwice()
            {
                task.Finish();
                Assert.Catch(() => task.Finish());
            }
        }
    }
}
