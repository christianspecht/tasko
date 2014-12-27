using System;
using System.Collections.Generic;
using Tasko.Server.Properties;

namespace Tasko.Server.Models
{
    /// <summary>
    /// A Tasko task
    /// </summary>
    public class Task
    {
        private string description;

        /// <summary>
        /// Creates a new task
        /// </summary>
        /// <param name="description">Description</param>
        /// <param name="category">First category</param>
        public Task(string description, string category)
        {
            this.Description = description;
            this.AddCategory(category);
            this.UpdateCreated();
        }

        /// <summary>
        /// internal constructor, needed for RavenDB
        /// </summary>
        protected Task()
        {
        }

        /// <summary>
        /// Id (internal, is set by RavenDB)
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Description (what to do)
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new TaskoInvalidInputException(Resources.DescriptionEmpty);
                }

                this.description = value;
                this.UpdateLastEdited();
            }
        }

        /// <summary>
        /// List of categories (readonly, use AddCategory to write)
        /// </summary>
        public List<string> Categories { get; private set; }

        /// <summary>
        /// date/time when the task was created
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// user who created the task
        /// </summary>
        public string CreatedBy { get; private set; }

        /// <summary>
        /// date/time when the task was last edited
        /// </summary>
        public DateTime LastEditedAt { get; private set; }

        /// <summary>
        /// user who last edited the task
        /// </summary>
        public string LastEditedBy { get; private set; }

        /// <summary>
        /// date/time when the task was finished
        /// </summary>
        public DateTime? FinishedAt { get; private set; }

        /// <summary>
        /// user who finished the task
        /// </summary>
        public string FinishedBy { get; private set; }


        /// <summary>
        /// Indicates whether the task is finished or not
        /// </summary>
        public bool IsFinished
        {
            get { return this.FinishedAt.HasValue; }
        }

        /// <summary>
        /// Adds a new category to this task
        /// </summary>
        /// <param name="category">The category</param>
        public void AddCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new TaskoInvalidInputException(Resources.CategoryEmpty);
            }

            if (this.Categories == null)
            {
                this.Categories = new List<string>();
            }

            this.Categories.Add(category);
            this.UpdateLastEdited();
        }

        /// <summary>
        /// Deletes the category from this task
        /// </summary>
        /// <param name="category">The category</param>
        public void DeleteCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new TaskoInvalidInputException(Resources.CategoryEmpty);
            }

            if (!this.Categories.Contains(category))
            {
                throw new TaskoInvalidInputException(Resources.CategoryDoesntExist);
            }

            if (this.Categories.Count == 1)
            {
                throw new TaskoInvalidInputException(Resources.CantDeleteLastCategory);
            }

            this.Categories.Remove(category);
            this.UpdateLastEdited();
        }

        /// <summary>
        /// Finishes the task
        /// </summary>
        public void Finish()
        {
            if (this.IsFinished)
            {
                throw new TaskoInvalidInputException(Resources.CanBeFinishedOnlyOnce);
            }

            UpdateFinished();
        }

        /// <summary>
        /// Reopens the task
        /// </summary>
        public void Reopen()
        {
            if (!this.IsFinished)
            {
                throw new TaskoInvalidInputException(Resources.CanBeReopenedOnlyWhenFinished);
            }

            UpdateFinished(true);
        }

        /// <summary>
        /// Helper function: sets CreatedAt (and LastEditedAt) to the current date/time
        /// </summary>
        private void UpdateCreated()
        {
            this.CreatedAt = DateTime.UtcNow;
            this.CreatedBy = Helper.CurrentUser;
            UpdateLastEdited();
        }

        /// <summary>
        /// Helper function: sets LastEditedAt to the current date/time
        /// </summary>
        private void UpdateLastEdited()
        {
            this.LastEditedAt = DateTime.UtcNow;
            this.LastEditedBy = Helper.CurrentUser;
        }

        /// <summary>
        /// Helper function: sets FinishedAt (and LastEditedAt) to the current date/time or to NULL
        /// </summary>
        /// <param name="delete">True to set to NULL</param>
        private void UpdateFinished(bool delete = false)
        {
            if (delete)
            {
                this.FinishedAt = null;
                this.FinishedBy = null;
            }
            else
            {
                this.FinishedAt = DateTime.UtcNow;
                this.FinishedBy = Helper.CurrentUser;
            }

            UpdateLastEdited();
        }
    }
}