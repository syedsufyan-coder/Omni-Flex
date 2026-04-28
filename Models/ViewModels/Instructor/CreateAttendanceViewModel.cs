using System.ComponentModel.DataAnnotations;

namespace OmniFlex.Models.ViewModels.Instructor
{
    // Used in your ViewModel

    public class AttendanceEntry
    {
        public int AttendanceId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    // Used for Frontend to Backend AJAX requests
    public class BulkAddAttendanceRequest
    {
        public string CourseId { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public decimal Duration { get; set; }
        public string DefaultStatus { get; set; } = "P";
    }

    public class UpdateAttendanceStatusRequest
    {
        public int AttendanceId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}