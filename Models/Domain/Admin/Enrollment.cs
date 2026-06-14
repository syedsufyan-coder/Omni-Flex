namespace OmniFlex.Models.Domain.Admin
{
    public class Enrollment
    {
        public int EnrollId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string OfferingId { get; set; } = string.Empty;
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = "Registered";
        public string Grade { get; set; } = string.Empty;
    }
}
