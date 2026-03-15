namespace OmniFlex.Models.Domain.Admin
{
    public class Section
    {
        public string SectionId { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string SemesterId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public int Seats { get; set; }
        public string? SectionLabel { get; set; }
        public string? RoomNo { get; set; }
        public string? TimeSlot { get; set; }
    }
}
