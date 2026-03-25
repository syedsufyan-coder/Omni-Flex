using System;
using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class ToDoViewModel
    {
        public List<ToDoItem> AssignedItems { get; set; }
        public List<ToDoItem> MissingItems { get; set; }
        public List<ToDoItem> DoneItems { get; set; }
    }

    public class ToDoItem
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string CourseColorClass { get; set; }
        public string AssignmentTitle { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }
}
