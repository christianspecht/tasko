using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tasko.Server.Models;

namespace Tasko.Server.Controllers
{
    public class TasksController : RavenController
    {
        public IEnumerable<Task> Get()
        {
            var tasks = this.RavenSession.Query<Task>().OrderBy(t => t.Description);
            return tasks;
        }

        public IEnumerable<Task> Get(string category)
        {
            var tasks = this.RavenSession.Query<Task>().Where(t => t.Categories.Contains(category)).OrderBy(t => t.Description);
            return tasks;
        }

        public Task Get(int id)
        {
            var task = this.RavenSession.Load<Task>("tasks/" + id.ToString()); 

            if (task == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return task;
        }

        public HttpResponseMessage Post(TaskCreateDto dto)
        {
            if (dto == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var task = new Task(dto.Description, dto.Category);
            this.RavenSession.Store(task);

            var resp = Request.CreateResponse<Task>(HttpStatusCode.Created, task);
            string uri = Url.Link("DefaultAPI", new { id = task.Id });
            resp.Headers.Location = new Uri(uri);

            return resp;
        }

        [ActionName("Finish")]
        [HttpPost]
        public HttpResponseMessage FinishTask(int id)
        {
            var task = this.RavenSession.Load<Task>("tasks/" + id.ToString());

            if (task == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            task.Finish();

            return Request.CreateResponse<Task>(HttpStatusCode.OK, task);
        }

        [ActionName("Reopen")]
        [HttpPost]
        public HttpResponseMessage ReopenTask(int id)
        {
            var task = this.RavenSession.Load<Task>("tasks/" + id.ToString());

            if (task == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            task.Reopen();

            return Request.CreateResponse<Task>(HttpStatusCode.OK, task);
        }


    }
}