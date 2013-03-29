using System.Collections.Generic;

namespace Tasko.Server.Models
{
    /// <summary>
    /// A Tasko task
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Id (internal, is set by RavenDB)
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Description (what to do)
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// List of categories (readonly, use AddCategory to write)
        /// </summary>
        public List<string> Categories { get; private set; }

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
        /// Adds a new category to this task
        /// </summary>
        /// <param name="category">The category</param>
        public void AddCategory(string category)
        {
            if (this.Categories == null)
            {
                this.Categories = new List<string>();
            }

            this.Categories.Add(category);
        }
    }
}