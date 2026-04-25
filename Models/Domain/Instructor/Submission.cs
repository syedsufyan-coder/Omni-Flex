namespace OmniFlex.Models.Domain.Instructor
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        public int AssignmentId { get; set; }
        public int EnrollmentId { get; set; }
        public DateTime SubmitDate { get; set; } = DateTime.Now;
        public decimal? ObtainedMarks { get; set; }
        public decimal? ObtainedWeightage { get; set; }
        public bool IsLate { get; set; } = false;
        public string? GradedBy { get; set; }
        public DateTime? GradedAt { get; set; }
        public bool Locked { get; set; } = false;
    }
}