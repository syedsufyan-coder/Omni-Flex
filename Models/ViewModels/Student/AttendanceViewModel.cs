using System;
using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class AttendanceViewModel
    {
        public List<CourseAttendance> Courses { get; set; }
    }

    public class CourseAttendance
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Section { get; set; }
        public int TotalClasses { get; set; }
        public int ClassesAttended { get; set; }
        public int ClassesMissed { get; set; }
        public double AttendancePercentage { get; set; }
        public List<AttendanceRecord> Records { get; set; }
    }

    public class AttendanceRecord
    {
        public DateTime Date { get; set; }
        public string Day { get; set; }
        public string Status { get; set; }
    }
}
