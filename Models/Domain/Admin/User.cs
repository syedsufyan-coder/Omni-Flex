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
        public int? Batch { get; set; }
        public string? Degree { get; set; }

        // Instructor and Admin only
        public string? Designation { get; set; }
        public string? OfficeRoom { get; set; }
        public string? Specialization { get; set; }
        public string? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}