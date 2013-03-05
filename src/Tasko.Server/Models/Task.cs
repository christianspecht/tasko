using System.Collections.Generic;

namespace Tasko.Server.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
    }
}