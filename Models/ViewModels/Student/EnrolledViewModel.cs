using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class EnrolledViewModel
    {
        public List<ClassCard>? ActiveClasses { get; set; }
    }

    public class ClassCard
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public int Batch { get; set; }
        public string BannerColorClass { get; set; } = "bg-primary"; // Default color class
        public bool IsArchived { get; set; }
    }
}
