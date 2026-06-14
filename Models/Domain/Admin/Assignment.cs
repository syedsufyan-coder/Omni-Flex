namespace OmniFlex.Models.Domain.Admin
{
    public class Assignment
    {
        public string AssignmentId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string OfferingId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public int TotalMarks { get; set; }
        public decimal ActualWtg { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
