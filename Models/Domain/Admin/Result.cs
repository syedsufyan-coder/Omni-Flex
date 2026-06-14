namespace OmniFlex.Models.Domain.Admin
{
    public class Result
    {
        public int ResultId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string OfferingId { get; set; } = string.Empty;
        public string? FinalGrade { get; set; }
        public decimal? FinalPercentage { get; set; }
    }
}
