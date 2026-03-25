namespace OmniFlex.Models.Domain.Student
{
    public class Result
    {
        public long ResultId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string? FinalGrade { get; set; }
        public decimal? FinalPercentage { get; set; }
    }
}
