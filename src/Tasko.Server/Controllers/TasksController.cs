using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tasko.Server.Models;
using Tasko.Server.Properties;

namespace Tasko.Server.Controllers
{
    public class TasksController : RavenController
    {
        // GET /api/tasks
        [ActionName("TaskInfo")]
        public IEnumerable<Task> Get()
        {
            var tasks = this.RavenSession.Query<Task>().OrderBy(t => t.Description);
            return tasks;
        }

        // GET /api/tasks?category={category}
        [ActionName("TaskInfo")]
        public IEnumerable<Task> Get(string category)
        {
            var tasks = this.RavenSession.Query<Task>().Where(t => t.Categories.Contains(category)).OrderBy(t => t.Description);
            return tasks;
        }

        // GET /api/tasks?finished={finished}
        [ActionName("TaskInfo")]
        public IEnumerable<Task> Get(bool finished)
        {
            var tasks = this.RavenSession.Query<Task>().Where(t => t.IsFinished == finished).OrderBy(t => t.Description);
            return tasks;
        }

        // GET /api/tasks?category={category}&finished={finished}
        [ActionName("TaskInfo")]
        public IEnumerable<Task> Get(string category, bool finished)
        {
            var tasks = this.RavenSession.Query<Task>().Where(t => t.Categories.Contains(category) && t.IsFinished == finished).OrderBy(t => t.Description);
            return tasks;
        }

        // GET /api/tasks/{id}
        [ActionName("TaskInfo")]
        public Task Get(int id)
        {
            var task = this.LoadTaskById(id);
            return task;
        }

        // POST /api/tasks
        [ActionName("TaskInfo")]
        public HttpResponseMessage Post(TaskCreateDto dto)
        {
            if (dto == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.MissingBody);
            }

            var task = new Task(dto.Description, dto.Category);
            this.RavenSession.Store(task);

            var resp = Request.CreateResponse<Task>(HttpStatusCode.Created, task);
            string uri = Url.Link("DefaultAPI", new { id = task.Id });
            resp.Headers.Location = new Uri(uri);

            return resp;
        }

        // POST /api/tasks/{id}/finish
        [ActionName("Finish")]
        [HttpPost]
        public HttpResponseMessage FinishTask(int id)
        {
            var task = this.LoadTaskById(id);

            task.Finish();

            return Request.CreateResponse<Task>(HttpStatusCode.OK, task);
        }

        // POST /api/tasks/{id}/reopen
        [ActionName("Reopen")]
        [HttpPost]
        public HttpResponseMessage ReopenTask(int id)
        {
            var task = this.LoadTaskById(id);

            task.Reopen();

            return Request.CreateResponse<Task>(HttpStatusCode.OK, task);
        }

        // PUT /api/tasks/{id}/description
        [ActionName("Description")]
        [HttpPut]
        public HttpResponseMessage ChangeDescription(int id, ChangeDescriptionDto dto)
        {
            if (dto == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.MissingBody);
            }

            var task = this.LoadTaskById(id);

            task.Description = dto.Description;

            return Request.CreateResponse<Task>(HttpStatusCode.OK, task);
        }

        // GET /api/tasks/{id}/categories
        [ActionName("Categories")]
        public IEnumerable<string> GetCategories(int id)
        {
            var task = this.LoadTaskById(id);
            return task.Categories;
        }

        // POST /api/tasks/{id}/categories
        [ActionName("Categories")]
        public HttpResponseMessage PostNewCategory(int id, PostNewCategoryDto dto)
        {
            if (dto == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.MissingBody);
            }

            var task = this.LoadTaskById(id);
            task.AddCategory(dto.Category);

            return Request.CreateResponse<IEnumerable<string>>(HttpStatusCode.Created, task.Categories);
        }

        // DELETE /api/tasks/{id}/categories/{category}
        [ActionName("Categories")]
        public HttpResponseMessage DeleteCategory(int id, string category)
        {
            var task = this.LoadTaskById(id);
            task.DeleteCategory(category);

            return Request.CreateResponse<IEnumerable<string>>(HttpStatusCode.OK, task.Categories);
        }

        /// <summary>
        /// helper method, loads task from DB
        /// </summary>
        private Task LoadTaskById(int id)
        {
            var task = this.RavenSession.Load<Task>("tasks/" + id.ToString());

            if (task == null)
            {
                var resp = Request.CreateErrorResponse(HttpStatusCode.NotFound, String.Format(Resources.TaskDoesntExist, id));
                throw new HttpResponseException(resp);
            }

            return task;
        }
    }
}