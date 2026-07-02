namespace OmniFlex.Models.Domain.Admin
{
    public class SectionOfferings
    {
        public string OfferingId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string SemesterId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
        public int Searts { get; set; }
    }
}