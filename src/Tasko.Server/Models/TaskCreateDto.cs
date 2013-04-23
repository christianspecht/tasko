
namespace Tasko.Server.Models
{
    /// <summary>
    /// DTO to post new tasks
    /// </summary>
    public class TaskCreateDto
    {
        public string Description { get; set; }
        public string Category { get; set; }
    }
}