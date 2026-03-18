using System;
namespace OmniFlex.Models.DTOs{
    public class SectionsDto {
        public string SectionId { get; set; } = string.Empty;
        public string SectionLabel { get; set; } = string.Empty;
        public string Degree { get; set; }       = string.Empty;
        public int EnrolledStudents { get; set; }
        public string CrFirstName { get; set; }  = string.Empty;
        public string CrLastName { get; set; }   = string.Empty;
        public string Department {get; set;} = string.Empty;
        public int Batch { get; set; }
    }
}
