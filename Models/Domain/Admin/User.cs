namespace OmniFlex.Models.Domain.Admin
{
    public class User
    {
        public string UserId { get; set; } = string.Empty;
        public string DeptId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";

        // Student/TA only
        public int? Batch { get; set; }
        public string? Degree { get; set; }
        public string? SectionName { get; set; }

        // Instructor only
        public string? Designation { get; set; }
        public string? OfficeRoom { get; set; }
        public string? Specialization { get; set; }
    }
}
