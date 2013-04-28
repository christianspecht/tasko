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
        Task[] tasks = new Task[]
        {
            new Task("task1", "CatA"),
            new Task("task2", "CatB")
        };

        public IEnumerable<Task> Get()
        {
            return tasks;
        }

        public IEnumerable<Task> Get(string category)
        {
            return tasks.Where(t => t.Categories.Any(c => c.Contains(category)));
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
            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}