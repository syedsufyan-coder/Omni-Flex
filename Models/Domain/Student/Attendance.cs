namespace OmniFlex.Models.Domain.Student
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public string EnrollId { get; set; } = string.Empty;
        public DateTime AttendanceDate { get; set; }
        public char Status { get; set; }
        public required string MarkedBy { get; set; }
    }
}