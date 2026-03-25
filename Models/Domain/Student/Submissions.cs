namespace OmniFlex.Models.Domain.Student
{
    public class Submissions
    {
        public long SubmissionId { get; set; }
        public string AssignmentId { get; set; } = string.Empty;
        public string EnrollId { get; set; } = string.Empty;
        public DateTime SubmitDate { get; set; }
        public double ObtainedMarks { get; set; }
        public double ObtainedWtg { get; set; }
        public char IsLate { get; set; }
        public required string GradedBy { get; set; }
        public DateTime GradedAt { get; set; }
        public char IsLocked { get; set; }
    }
}