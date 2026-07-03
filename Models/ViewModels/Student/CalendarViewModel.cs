using System;
using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class CalendarViewModel
    {
        public int CurrentYear { get; set; }
        public int CurrentMonth { get; set; }
        public List<CalendarEvent> Events { get; set; } = new();
    }

    public class CalendarEvent
    {
        public string Title { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
