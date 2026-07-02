using System;
using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Instructor
{
    public class InstructorCoursePostViewModel
    {
        public int PostId { get; set; }
        public string PostedBy { get; set; } = string.Empty;
        public string PostType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public string PostedByName { get; set; } = string.Empty;
        public List<string> AttachedFileNames { get; set; } = new();
    }
}
