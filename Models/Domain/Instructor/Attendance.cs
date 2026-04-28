namespace OmniFlex.Models.Domain.Instructor
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int EnrollId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; } = string.Empty; // "P" for Present, "A" for Absent, "L" for Late
        public string MarkedBy { get; set; } = string.Empty; // User ID of the person who marked attendance
        public decimal Duration { get; set; } // Duration in hours, default is 1.00
    }
}