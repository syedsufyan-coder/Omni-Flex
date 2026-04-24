namespace OmniFlex.Models.DTOs
{
    public class GradeGridDTO
    {
        public string AssignmentTitle { get; set; } = string.Empty;
        public int AssignmentId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public decimal MaxMarks { get; set; }
        public DateTime? SubmitDate { get; set; }
        public decimal? ObtainedMarks { get; set; }
    }
}