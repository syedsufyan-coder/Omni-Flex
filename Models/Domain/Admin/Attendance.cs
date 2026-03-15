namespace OmniFlex.Models.Domain.Admin
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int EnrollId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public char Status { get; set; } // P / A / L
        public string MarkedBy { get; set; } = string.Empty;
    }
}
