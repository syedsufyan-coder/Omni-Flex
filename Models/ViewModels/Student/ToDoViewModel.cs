using System;
using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class ToDoViewModel
    {
        public List<ToDoItem> AssignedItems { get; set; } = new();
        public List<ToDoItem> MissingItems { get; set; } = new();
        public List<ToDoItem> DoneItems { get; set; } = new();
    }

    public class ToDoItem
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string CourseColorClass { get; set; } = string.Empty;
        public string AssignmentTitle { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
