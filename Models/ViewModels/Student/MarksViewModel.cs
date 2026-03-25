using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class MarksViewModel
    {
        public List<CourseMarks> Courses { get; set; }
    }

    public class CourseMarks
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Section { get; set; }
        public string EnrollmentStatus { get; set; }
        public List<AssessmentMark> Assessments { get; set; }
        public double TotalObtained { get; set; }
        public double TotalMax { get; set; }
    }

    public class AssessmentMark
    {
        public string Component { get; set; }
        public double ObtainedMarks { get; set; }
        public double TotalMarks { get; set; }
        public string Remarks { get; set; }
        public double Percentage => TotalMarks > 0 ? (ObtainedMarks / TotalMarks) * 100 : 0;
    }
}
