namespace OmniFlex.Models.Domain.Instructor
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        public string OfferingId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DeliveryMode { get; set; } = "Online";
        public DateTime? DueDate { get; set; }
        public string? Category { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal? ActualWeightage { get; set; }
        public bool IsGraded { get; set; } = true;
        public string? GradingGroup { get; set; }
        public int? CountBestOf { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}