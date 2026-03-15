namespace OmniFlex.Models.Domain.Admin
{
    public class CoursePost
    {
        public string PostId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string PostedBy { get; set; } = string.Empty;
        public string PostType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
