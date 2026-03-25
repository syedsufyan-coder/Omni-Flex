using System;
namespace OmniFlex.Models.DTOs
{
    public class SectionsDto
    {
        public string SectionId { get; set; } = string.Empty;
        public string SectionLabel { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public int EnrolledStudents { get; set; }
        public string CrFirstName { get; set; } = string.Empty;
        public string CrLastName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int Batch { get; set; }
    }

    public class CourseFilterRequest
    {
        public string? DeptId { get; set; }
        public string? CourseType { get; set; }
        public string? CourseCat { get; set; }
        public int? CreditHrs { get; set; }
        public string? TeacherId { get; set; }
        public string? SectionId { get; set; }
        public string? PreReqId { get; set; }
    }
}
