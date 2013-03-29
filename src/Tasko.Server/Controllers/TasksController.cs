using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Tasko.Server.Models;

namespace Tasko.Server.Controllers
{
    public class TasksController : ApiController
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
            var task = tasks.SingleOrDefault(t => t.Id == id);

            if (task == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return task;
        }
    }
}