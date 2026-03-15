namespace OmniFlex.Models.ViewModels.Admin
{
    public class SectionViewModel
    {
        public string SectionId { get; set; } = string.Empty;
        public string SectionLabel { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public string? RoomNo { get; set; }
        public string? TimeSlot { get; set; }
        public int Enrolled { get; set; }
        public int Seats { get; set; }
        public string Semester { get; set; } = string.Empty;
        public List<string> TaNames { get; set; } = new();
    }
}
