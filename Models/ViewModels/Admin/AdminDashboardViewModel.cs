namespace OmniFlex.Models.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalTAs { get; set; }
        public int ActiveCourses { get; set; }
        public List<RecentActivityItem> RecentActivity { get; set; } = new();
        public List<CourseSummaryItem> CoursesSummary { get; set; } = new();
        public List<CourseViewModel> Courses { get; set; } = new();
    }

    public class RecentActivityItem
    {
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
    }

    public class CourseSummaryItem
    {
        public string CourseId { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Instructor { get; set; } = string.Empty;
        public int Enrolled { get; set; }
        public int Seats { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
