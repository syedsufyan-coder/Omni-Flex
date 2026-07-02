using System;

namespace OmniFlex.Models.DTOs{
    public class CourseDto
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string CreditHours { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public string CourseCat { get; set; } = string.Empty;
        public string? PreRequisite { get; set; }

    }
}
