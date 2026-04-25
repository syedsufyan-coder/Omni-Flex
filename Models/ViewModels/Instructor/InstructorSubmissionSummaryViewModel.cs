using System;

namespace OmniFlex.Models.ViewModels.Instructor
{
    public class InstructorSubmissionSummaryViewModel
    {
        public int SubmissionId { get; set; }
        public int EnrollId { get; set; }
        public int AssignmentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public DateTime? SubmitDate { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public string IsLate { get; set; } = string.Empty;
        public bool IsLocked { get; set; } = false;
        public bool IsGraded => ObtainedMarks.HasValue;
    }
}
