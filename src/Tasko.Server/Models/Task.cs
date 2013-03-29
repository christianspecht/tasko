using System.Collections.Generic;
using System;

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
                    throw new ArgumentOutOfRangeException("description can't be null or empty");
                }

                this.description = value;
            }
        }

        /// <summary>
        /// List of categories (readonly, use AddCategory to write)
        /// </summary>
        public List<string> Categories { get; private set; }

        /// <summary>
        /// Adds a new category to this task
        /// </summary>
        /// <param name="category">The category</param>
        public void AddCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentOutOfRangeException("category can't be null or empty");
            }

            if (this.Categories == null)
            {
                this.Categories = new List<string>();
            }

            this.Categories.Add(category);
        }
    }
}