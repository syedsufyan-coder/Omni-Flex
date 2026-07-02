namespace OmniFlex.Models.Domain.Instructor
{
    public class CoursePost
    {
        public int PostId { get; set; }
        public string OfferingId { get; set; } = string.Empty;
        public string PostedBy { get; set; } = string.Empty;
        public string PostType { get; set; } = string.Empty; // Announcement, Material, etc.
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}