using System;

namespace OmniFlex.Models.DTOs
{
    public class AttendanceDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public int Batch { get; set; }
        public string Degree { get; set; } = string.Empty;
        public DateTime Attendance_Date { get; set; }
        public string Attendance_Day { get; set; } = string.Empty;
        public decimal Duration { get; set; }
        public string Status { get; set; } = string.Empty;

    }
}
