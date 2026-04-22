using System.Collections.Generic;
using OmniFlex.Models.DTOs;

namespace OmniFlex.Models.ViewModels.Student
{
    public class CourseDetailsViewModel
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public int Batch { get; set; }
        public string ActiveTab { get; set; } = "stream";
        public string CourseDescription { get; set; } = string.Empty;
        public int UpcomingDueCount { get; set; }
        public List<string> ClassworkItems { get; set; } = new List<string>();
        public List<string> People { get; set; } = new List<string>();
        
        // Only working DTO right now
        public CoursePostDto PostData { get; set; } = new CoursePostDto();
    }
}
