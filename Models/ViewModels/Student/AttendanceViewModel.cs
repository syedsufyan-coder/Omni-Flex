using System;
using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Student
{
    public class AttendanceViewModel
    {
        public List<CourseAttendance> Courses { get; set; } = new();
    }

    public class CourseAttendance
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public int TotalClasses { get; set; }
        public int ClassesAttended { get; set; }
        public int ClassesMissed { get; set; }
        public double AttendancePercentage { get; set; }
        public List<AttendanceRecord> Records { get; set; } = new();
    }

    public class AttendanceRecord
    {
        public DateTime Attendance_Date { get; set; }
        public string Attendance_Day { get; set; } = string.Empty;
        public double Duration { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
