namespace OmniFlex.Models.Domain.Student
{
    public class User
    {
        public string UserId { get; set; } = string.Empty;
        public string DeptId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";

        // Student only
        public int? Batch { get; set; }
        public string? Degree { get; set; }

        // TA only
        public string? TaPasswordHash {get; set;}

        // Instructor only
        public string? Designation { get; set; }
        public string? OfficeRoom { get; set; }
        public string? Specialization { get; set; }

    }
}
