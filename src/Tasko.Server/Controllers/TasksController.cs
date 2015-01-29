using Raven.Client;
using Raven.Client.Linq;
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

        // POST /api/tasks/search
        [ActionName("Search")]
        [HttpPost]
        public HttpResponseMessage CreateSearch(CreateSearchDto dto)
        {
            if (dto.PageNumber == 0)
            {
                dto.PageNumber = 1;
            }

            if (dto.PageSize == 0)
            {
                dto.PageSize = 10;
            }

            RavenQueryStatistics stats;
            IRavenQueryable<Task> tasks = this.RavenSession.Query<Task>().Statistics(out stats);

            if (!string.IsNullOrWhiteSpace(dto.Category))
            {
                tasks = tasks.Where(t => t.Categories.Contains(dto.Category));
            }

            if (dto.Finished != null)
            {
                tasks = tasks.Where(t => t.IsFinished == dto.Finished);
            }

            tasks = tasks.Skip((dto.PageNumber - 1) * dto.PageSize).Take(dto.PageSize) as IRavenQueryable<Task>;

            tasks = tasks.OrderBy(t => t.Description);

            int count = stats.TotalResults;  // TODO: do something with it

            return Request.CreateResponse<IEnumerable<Task>>(HttpStatusCode.OK, tasks.ToList());
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