using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Instructor
{
    public class InstructorClassroomViewModel
    {
        public string OfferingId { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string InstructorId { get; set; } = string.Empty;
        public string ActiveTab { get; set; } = "stream";

        public List<InstructorCoursePostViewModel> Posts { get; set; } = new();
        public List<InstructorAssignmentViewModel> Assignments { get; set; } = new();
        public int EnrolledCount { get; set; }
    }
}
