namespace OmniFlex.Models.ViewModels.Admin
{
    public class AnnouncementViewModel
    {
        public string AnnouncementId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string PostedByName { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool IsPinned { get; set; }
        public string? SectionId { get; set; }
        public DateTime PostDate { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
    }
}
