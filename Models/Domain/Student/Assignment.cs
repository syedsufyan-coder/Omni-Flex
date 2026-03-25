namespace OmniFlex.Models.Domain.Student
{
    public class Assignment
    {
        public string AssignmentId { get; set; } = string.Empty;
        public string OfferingId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public required string DeliveryMode {get; set;}
        public DateTime DueDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public double TotalMarks { get; set; }
        public double ActualWtg { get; set; }
        public char IsGraded {get; set;}
        public string? GradingGroup {get; set;}
        public int CountBestOf {get; set;}
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
