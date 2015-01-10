
namespace Tasko.Server.Models
{
    /// <summary>
    /// DTO for: POST /api/tasks/search
    /// </summary>
    public class CreateSearchDto
    {
        public string Category { get; set; }
        public bool? Finished { get; set; }
        public int PageNumber { get; set; }
        public byte PageSize { get; set; }
    }
}