namespace OmniFlex.Models.DTOs
{
    public class AttendanceFlatDto
    {
        // Student Info
        public int EnrollId { get; set; }
        public string StudentId { get; set; } = string.Empty; // The User_ID/Roll No
        public string FullName { get; set; } = string.Empty;

        // Attendance Info (Can be null if no record exists for the date/month)
        public int AttendanceId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string Status { get; set; } = string.Empty; // "P", "A", "L" or "" for no record
        public decimal Duration { get; set; } // Duration in hours, default is 1.00
    }
}