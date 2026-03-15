namespace OmniFlex.Models.Domain.Admin
{
    public class Submission
    {
        public string SubmissionId { get; set; } = string.Empty;
        public string AssignmentId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public DateTime SubmitDate { get; set; }
        public decimal? ObtainedMarks { get; set; }
        public decimal? ObtainedWtg { get; set; }
        public int IsLate { get; set; }
        public string? GradedBy { get; set; }
        public DateTime? GradedAt { get; set; }
        public int Locked { get; set; }
    }
}
