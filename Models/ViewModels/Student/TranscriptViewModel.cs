using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class TranscriptViewModel
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string DegreeProgram { get; set; } = string.Empty;
        public int Batch { get; set; }
        public List<SemesterSection>? Semesters { get; set; }
    }

    public class SemesterSection
    {
        public string SemesterName { get; set; } = string.Empty;
        public decimal SemCrAtt { get; set; }
        public decimal SemCrErnd { get; set; }
        public decimal SGPA { get; set; }
        public decimal CGPA { get; set; }
        public List<TranscriptCourseRow> Courses { get; set; } = new();
    }

    public class TranscriptCourseRow
    {
        public string Code { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public int Batch { get; set; }
        public string Degree { get; set; } = string.Empty;
        public decimal Credits { get; set; }
        public string Grade { get; set; } = string.Empty;
        public decimal Points { get; set; }
        public string Type { get; set; } = string.Empty; // Core, Elective, etc.
    }
}
