namespace OmniFlex.Models.Domain.Admin
{
    public class Announcement
    {
        public string AnnouncementId { get; set; } = string.Empty;
        public string PostedBy { get; set; } = string.Empty;
        public string? SectionId { get; set; } // NULL = system-wide
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Audience { get; set; } = "All";
        public int IsPinned { get; set; }
        public string Priority { get; set; } = "Normal";
        public DateTime PostDate { get; set; }
    }
}
