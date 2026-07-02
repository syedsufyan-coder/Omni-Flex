namespace OmniFlex.Models.Domain.Instructor
{
    public class Exam
    {
        public int EntryId { get; set; }
        public int AssignmentId { get; set; }
        public int EnrollmentId { get; set; }
        public decimal MarksObtained { get; set; }
        public DateTime? ExamDate { get; set; }
        public string EnteredBy { get; set; } = string.Empty;
        public DateTime EnteredAt { get; set; } = DateTime.Now;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Remarks { get; set; }
    }
}