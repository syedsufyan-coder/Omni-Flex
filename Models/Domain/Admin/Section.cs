namespace OmniFlex.Models.Domain.Admin
{
    public class Section
    {
        public string SectionId { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string SectionLabel { get; set; } = string.Empty;
        public string DegreeProgram { get; set; } = string.Empty;
        public int BatchYear { get; set; }
        public string? CrId { get; set; }
    }
}