    namespace OmniFlex.Models.DTOs{
    public class AssignedClassesDTO
    {
        public string OfferingId { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public int Batch { get; set; }
        public string Degree { get; set; } = string.Empty;
        public string CreditHrs { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public int EnrolledStudents { get; set; }
        public string Status { get; set; } = string.Empty;
    }
    }