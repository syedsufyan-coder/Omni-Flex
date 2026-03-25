using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class TranscriptViewModel
    {
        public string RollNo { get; set; }
        public string StudentName { get; set; }
        public string Degree { get; set; }
        public string Batch { get; set; }
        public string Program { get; set; }
        public List<SemesterTranscript> Semesters { get; set; }
    }

    public class SemesterTranscript
    {
        public string SemesterLabel { get; set; }
        public int CreditHoursAttempted { get; set; }
        public int CreditHoursEarned { get; set; }
        public double SGPA { get; set; }
        public double CGPA { get; set; }
        public List<TranscriptCourse> Courses { get; set; }
    }

    public class TranscriptCourse
    {
        public string Code { get; set; }
        public string CourseName { get; set; }
        public string Section { get; set; }
        public int CreditHours { get; set; }
        public string Grade { get; set; }
        public double GradePoints { get; set; }
        public string Type { get; set; }
    }
}
