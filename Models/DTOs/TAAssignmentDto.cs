namespace OmniFlex.Models.DTOs
{
    public class TAAssignmentDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string CourseId { get; set; } = string.Empty;
        public string SemesterId { get; set; } = string.Empty;
        public string AssignedBy { get; set; } = string.Empty;
        public int Batch { get; set; }
    }
}