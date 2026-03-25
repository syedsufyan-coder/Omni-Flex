namespace OmniFlex.Models.Domain.Admin
{
    public class Enrollment
    {
        public long EnrollId { get; set; }
        public string SectionId { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string OfferingId { get; set; } = string.Empty;
    }
}