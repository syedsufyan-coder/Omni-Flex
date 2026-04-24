using System;
using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Instructor
{
    public class InstructorAssignmentViewModel
    {
        public int AssignmentId { get; set; }
        public string OfferingId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DeliveryMode { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal TotalMarks { get; set; }
        public decimal ActualWtg { get; set; }
        public string IsGraded { get; set; } = string.Empty;
        public string GradingGroup { get; set; } = string.Empty;
        public int? CountBestOf { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SubmissionCount { get; set; }
        public int TotalEnrolled { get; set; }
        public List<InstructorSubmissionSummaryViewModel>? Submissions { get; set; } = new();
    }
}
