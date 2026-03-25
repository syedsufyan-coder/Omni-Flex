using System;
using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class CalendarViewModel
    {
        public int CurrentYear { get; set; }
        public int CurrentMonth { get; set; }
        public List<CalendarEvent> Events { get; set; }
    }

    public class CalendarEvent
    {
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public string CourseCode { get; set; }
        public string ColorClass { get; set; }
        public string Type { get; set; }
    }
}
