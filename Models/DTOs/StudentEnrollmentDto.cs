namespace OmniFlex.Models.DTOs
{
    public class StudentEnrollmentDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public int Batch { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}