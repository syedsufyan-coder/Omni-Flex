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

    public class OnsiteExamViewModel
    {
        public int AssignmentId { get; set; }
        public decimal TotalMarks { get; set; }

        public List<OnsiteStudentRow> Students { get; set; } = new();
    }

    public class OnsiteStudentRow
    {
        public int EnrollmentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;

        // nullable because may not exist yet
        public int? EntryId { get; set; }

        public decimal? MarksObtained { get; set; }
        public DateTime? ExamDate { get; set; }
        public string? Remarks { get; set; }

        public bool IsGraded => EntryId.HasValue;
    }
}