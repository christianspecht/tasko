using NUnit.Framework;
using System;
using System.Security.Principal;
using System.Threading;
using Tasko.Server.Models;

namespace Tasko.Tests.Unit
{
    public class TaskTests
    {
        private Task task;
        private DateTime lastEditedAt;

        [SetUp]
        public void Setup()
        {
            // set current user                   
            var newIdentity = new GenericIdentity("newuser");
            var newPrincipal = new GenericPrincipal(newIdentity, null);
            Tasko.Server.Helper.SetCurrentUser(() => newPrincipal);

            this.task = new Task("foo", "cat1");

            // save value for tests (to be able to tell if the value was changed after creation)
            this.lastEditedAt = this.task.LastEditedAt;

            // set current user                   
            var editIdentity = new GenericIdentity("edituser");
            var editPrincipal = new GenericPrincipal(editIdentity, null);
            Tasko.Server.Helper.SetCurrentUser(() => editPrincipal);
        }

        /// <summary>
        /// helper method: pause for a few ms
        /// </summary>
        /// <remarks>
        /// To make sure that the "last edited" value is actually different when set a second time.
        /// </remarks>
        public void Wait()
        {
            Thread.Sleep(30);
        }

        [TestFixture]
        public class Constructor : TaskTests
        {
            [Test]
            public void DescriptionWasSet()
            {
                Assert.AreEqual("foo", task.Description);
            }

            [Test]
            public void ThrowsWhenDescriptionIsEmpty()
            {
                Assert.Catch(() => new Task("", "cat1"));
            }

            [Test]
            public void CategoryWasAdded()
            {
                Assert.AreEqual(1, task.Categories.Count);
                Assert.AreEqual("cat1", task.Categories[0]);
            }

            [Test]
            public void ThrowsWhenCategoryIsEmpty()
            {
                Assert.Catch(() => new Task("foo", ""));
            }

            [Test]
            public void CreatedAtWasSet()
            {
                Assert.AreNotEqual(DateTime.MinValue, task.CreatedAt);
                Assert.AreEqual("newuser", task.CreatedBy);
            }

            [Test]
            public void LastEditedWasSet()
            {
                Assert.AreNotEqual(DateTime.MinValue, task.LastEditedAt);
                Assert.AreEqual("newuser", task.LastEditedBy);
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
                Wait();
                task.Description = "bar";
                Assert.AreNotEqual(this.lastEditedAt, task.LastEditedAt);
                Assert.AreEqual("edituser", task.LastEditedBy);
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

                Assert.AreEqual(numberOfCategories + 1, task.Categories.Count);
                Assert.Contains("cat2", task.Categories);
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
                Wait();
                task.AddCategory("cat2");
                Assert.AreNotEqual(this.lastEditedAt, task.LastEditedAt);
                Assert.AreEqual("edituser", task.LastEditedBy);
            }
        }

        [TestFixture]
        public class DeleteCategory : TaskTests
        {
            [Test]
            public void CategoryWasDeleted()
            {
                task.AddCategory("cat2");

                task.DeleteCategory("cat1");
                Assert.AreEqual(1, task.Categories.Count);
                Assert.IsFalse(task.Categories.Contains("cat1"));
            }

            [Test]
            public void LastEditedChanges()
            {
                task.AddCategory("cat2");
                // save "last edited" again
                this.lastEditedAt = this.task.LastEditedAt;
                Wait();

                task.DeleteCategory("cat1");
                Assert.AreNotEqual(this.lastEditedAt, task.LastEditedAt);
            }

            [Test]
            public void ThrowsWhenCategoryIsEmpty()
            {
                Assert.Catch(() => task.DeleteCategory(null));
                Assert.Catch(() => task.DeleteCategory(""));
                Assert.Catch(() => task.DeleteCategory("   "));
            }

            [Test]
            public void ThrowsWhenCategoryDoesntExist()
            {
                task.AddCategory("cat2");

                Assert.Catch(() => task.DeleteCategory("foo"));
            }

            [Test]
            public void ThrowsWhenDeletingLastCategory()
            {
                Assert.Catch(() => task.DeleteCategory("cat1"));
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
                Assert.AreEqual("edituser", task.FinishedBy);
            }

            [Test]
            public void LastEditedIsSet()
            {
                Wait();
                task.Finish();
                Assert.AreNotEqual(this.lastEditedAt, task.LastEditedAt);
                Assert.AreEqual("edituser", task.LastEditedBy);
            }

            [Test]
            public void ThrowsWhenTaskIsFinishedTwice()
            {
                task.Finish();
                Assert.Catch(() => task.Finish());
            }
        }

        [TestFixture]
        public class Reopen : TaskTests
        {
            [Test]
            public void TaskIsNotFinished()
            {
                task.Finish();
                task.Reopen();

                Assert.That(!task.IsFinished);
            }

            [Test]
            public void FinishedAtIsNull()
            {
                task.Finish();
                task.Reopen();

                Assert.That(!task.FinishedAt.HasValue);
                Assert.IsNull(task.FinishedBy);
            }

            [Test]
            public void LastEditedIsSet()
            {
                task.Finish();
                
                // save LastEdited again and wait (to make sure that the value is different)
                this.lastEditedAt = this.task.LastEditedAt;
                Wait();

                task.Reopen();

                Assert.AreNotEqual(this.lastEditedAt, task.LastEditedAt);
                Assert.AreEqual("edituser", task.LastEditedBy);
            }

            [Test]
            public void ThrowsWhenUnfinishedTaskIsReopened()
            {
                Assert.Catch(() => task.Reopen());
            }

            [Test]
            public void ThrowsWhenTaskIsReopenedTwice()
            {
                task.Finish();
                task.Reopen();
                Assert.Catch(() => task.Reopen());
            }
        }
    }
}
