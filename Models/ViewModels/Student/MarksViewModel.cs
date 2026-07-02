using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class MarksViewModel
    {
        public List<CourseMarks> Courses { get; set; } = new();
    }

    public class CourseMarks
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string EnrollmentStatus { get; set; } = string.Empty;
        public List<AssessmentMark> Assessments { get; set; } = new();
        public double TotalObtained { get; set; }
        public double TotalMax { get; set; }
    }

    public class AssessmentMark
    {
        public string Component { get; set; } = string.Empty;
        public double ObtainedMarks { get; set; }
        public double TotalMarks { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public double Percentage => TotalMarks > 0 ? (ObtainedMarks / TotalMarks) * 100 : 0;
    }
}
