using System;

namespace OmniFlex.Models.DTOs
{
    public class AttendanceDto
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Section { get; set; }
        public int Batch { get; set; }
        public string Degree { get; set; }
        public DateTime Attendance_Date { get; set; }
        public string Attendance_Day { get; set; }
        public decimal Duration { get; set; }
        public string Status { get; set; }

    }
}
