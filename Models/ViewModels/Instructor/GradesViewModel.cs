namespace OmniFlex.Models.ViewModels.Instructor
{
    public class GradesViewModel
    {
        // The Headers (Columns)
        public List<AssignmentHeaderViewModel> Assignments { get; set; } = new();

        // The Rows
        public List<StudentGradeRowViewModel> StudentRows { get; set; } = new();

        // Class Averages for the second row
        public Dictionary<int, decimal?> AssignmentAverages { get; set; } = new();
    }

    public class AssignmentHeaderViewModel
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal MaxMarks { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class StudentGradeRowViewModel
    {
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        // Key: AssignmentId, Value: Obtained Marks
        public Dictionary<int, decimal?> Grades { get; set; } = new();
    }
}