namespace OmniFlex.Models.ViewModels.Admin
{
    public class SectionViewModel
    {
        public string SectionId { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string SectionLabel { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        // public string CourseName { get; set; } = string.Empty;
        //public string CourseId { get; set; } = string.Empty;
        // public string Instructor { get; set; } = string.Empty;
        // public string TeacherId { get; set; } = string.Empty;
        public int EnrolledStudents { get; set; }
        public string CrName { get; set; } = string.Empty; // Calculated: FIRST_NAME + LAST_NAME
        public string Department { get; set; } = string.Empty;
        public int SeatsLeft { get; set; }
        public int Seats { get; set; }
        public int BatchYear { get; set; }
    }
}
