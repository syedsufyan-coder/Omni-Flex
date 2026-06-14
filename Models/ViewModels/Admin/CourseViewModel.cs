namespace OmniFlex.Models.ViewModels.Admin
{
    public class CourseViewModel
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string DeptId { get; set; } = string.Empty;   
        public string DeptName { get; set; } = string.Empty; 
        public int CreditHrs { get; set; }
        public string CourseType { get; set; } = string.Empty;
        public string CourseCat { get; set; } = string.Empty;
        public string? PreReqId { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}