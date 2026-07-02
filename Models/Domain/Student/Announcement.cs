namespace OmniFlex.Models.Domain.Student
{
    public class Announcement
    {
        public long AnnouncementId { get; set; }
        public string PostedBy { get; set; } = string.Empty;
        public string AnnouncementType { get; set; } = string.Empty;
        public string OfferingId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public char IsPinned {get; set;}
        public string? Priority {get; set;}
        public long? ReferenceSubmissionId {get; set;}
        public DateTime PostDate { get; set; }

    }
}