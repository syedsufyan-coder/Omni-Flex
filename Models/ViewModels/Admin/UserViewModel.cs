namespace OmniFlex.Models.ViewModels.Admin
{
    public class UserViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Initials { get; set; } = string.Empty;
        public int? Batch { get; set; }
        public string? Degree { get; set; }
        public string? Designation { get; set; }
        public string? Specialization { get; set; }
    }
}
