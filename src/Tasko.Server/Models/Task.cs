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
            this.UpdateLastEdited();
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
                    throw new ArgumentOutOfRangeException(Resources.DescriptionEmpty);
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
        /// date/time when the task was last edited
        /// </summary>
        public DateTime LastEditedAt { get; private set; }

        /// <summary>
        /// Adds a new category to this task
        /// </summary>
        /// <param name="category">The category</param>
        public void AddCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentOutOfRangeException(Resources.CategoryEmpty);
            }

            if (this.Categories == null)
            {
                this.Categories = new List<string>();
            }

            this.Categories.Add(category);
            this.UpdateLastEdited();
        }

        /// <summary>
        /// Helper function: sets CreatedAt to the current date/time
        /// </summary>
        private void UpdateCreated()
        {
            this.CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Helper function: sets LastEditedAt to the current date/time
        /// </summary>
        private void UpdateLastEdited()
        {
            this.LastEditedAt = DateTime.UtcNow;
        }
    }
}