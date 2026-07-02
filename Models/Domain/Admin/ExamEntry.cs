namespace OmniFlex.Models.Domain.Admin
{
    public class ExamEntry
    {
        public long ExamEntryId { get; set; }
        public long AssignmentId { get; set; }
        public long EnrollId { get; set; }
        public double MarksObtained { get; set; } = 0;
        public DateTime ExamDate { get; set; }
        public string EnteredBy { get; set; } = string.Empty;
        public DateTime EnteredAt { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}