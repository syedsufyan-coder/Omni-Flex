using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class TranscriptViewModel
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string DegreeProgram { get; set; }
        public int Batch { get; set; }
        public List<SemesterSection>? Semesters { get; set; }
    }

    public class SemesterSection
    {
        public string SemesterName { get; set; }
        public decimal SemCrAtt { get; set; }
        public decimal SemCrErnd { get; set; }
        public decimal SGPA { get; set; }
        public decimal CGPA { get; set; }
        public List<TranscriptCourseRow> Courses { get; set; } = new();
    }

    public class TranscriptCourseRow
    {
        public string Code { get; set; }
        public string CourseName { get; set; }
        public string Section { get; set; }
        public int Batch { get; set; }
        public string Degree { get; set; }
        public decimal Credits { get; set; }
        public string Grade { get; set; }
        public decimal Points { get; set; }
        public string Type { get; set; } // Core, Elective, etc.
    }
}
