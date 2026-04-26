namespace OmniFlex.Models.Domain.Instructor
{
    public class Assignment
    {
        public string OfferingId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DeliveryMode { get; set; } = string.Empty; // "Online" or "InPerson"
        public string Category { get; set; } = string.Empty; // "Homework", "Quiz", "Exam", etc.
        public DateTime? DueDate { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal ActualWtg { get; set; } // Matches JS payload
        public string IsGraded { get; set; } = "Y";   // "Y" or "N"
        public string GradingGroup { get; set; } = string.Empty;
        public int? CountBestOf { get; set; }
    }
}