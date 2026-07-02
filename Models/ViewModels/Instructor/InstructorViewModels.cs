using System.Collections.Generic;

namespace OmniFlex.Models.ViewModels.Instructor
{
    public class InstructorDashboardViewModel
    {
        public string GreetingMessage { get; set; } = string.Empty;
        public InstructorProfileInfo Profile { get; set; } = new InstructorProfileInfo();
        public List<InstructorAssignedClassCard> AssignedClasses { get; set; } = new List<InstructorAssignedClassCard>();
        public InstructorWeeklySummary WeeklySummary { get; set; } = new InstructorWeeklySummary();
        public List<InstructorCourseCard>? Courses { get; set; } = new List<InstructorCourseCard>();
    }

    public class InstructorWeeklyCalendarViewModel
    {
        public string GreetingMessage { get; set; } = string.Empty;
        public InstructorProfileInfo Profile { get; set; } = new InstructorProfileInfo();
        public InstructorWeeklySummary Summary { get; set; } = new InstructorWeeklySummary();
        public List<InstructorCourseCard> AssignedCourses { get; set; } = new List<InstructorCourseCard>();
    }

    /*
    public class InstructorAttendanceViewModel
    {
        public string GreetingMessage { get; set; } = string.Empty;
        public InstructorProfileInfo Profile { get; set; } = new InstructorProfileInfo();
        public string SelectedSemester { get; set; } = string.Empty;
        public List<SelectOption> SemesterOptions { get; set; } = new List<SelectOption>();
        public string SelectedCourseId { get; set; } = string.Empty;
        public List<SelectOption> CourseOptions { get; set; } = new List<SelectOption>();
        public string SelectedSectionId { get; set; } = string.Empty;
        public List<SelectOption> SectionOptions { get; set; } = new List<SelectOption>();
        public string SelectedMonth { get; set; } = string.Empty;
        public List<SelectOption> MonthOptions { get; set; } = new List<SelectOption>();
        public int CurrentWeek { get; set; }
        public string SelectedDuration { get; set; } = string.Empty;
        public List<SelectOption> DurationOptions { get; set; } = new List<SelectOption>();
        public List<string> AttendanceHeaders { get; set; } = new List<string>();
        public List<InstructorAttendanceStudentRow> Students { get; set; } = new List<InstructorAttendanceStudentRow>();
    }*/

    public class InstructorAttendanceViewModel
    {
        public string InstructorName { get; set; } = string.Empty;
        public string SelectedCourseId { get; set; } = string.Empty;
        public string SelectedSectionId { get; set; } = string.Empty;
        public string SelectedMonth { get; set; } = string.Empty;
        public decimal SelectedDuration { get; set; }

        public List<SelectOption> CourseOptions { get; set; } = new();
        public List<SelectOption> SectionOptions { get; set; } = new();
        public List<SelectOption> MonthOptions { get; set; } = new();
        public List<SelectOption> DurationOptions { get; set; } = new();

        public List<InstructorAttendanceStudentRow> Students { get; set; } = new();
    }

    public class InstructorAttendanceStudentRow
    {
        public int SNo { get; set; }
        public int EnrollId { get; set; }
        public string RollNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // This list creates the dynamic columns (one for each date)
        public List<AttendanceRecordViewModel> Attendances { get; set; } = new();
    }

    public class AttendanceRecordViewModel
    {
        public int AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = "P";
    }

    public class InstructorProfileInfo
    {
        public string InstructorId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string OfficeRoom { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string MobileNo { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string HomePhone { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class InstructorAssignedClassCard
    {
        public string OfferingId { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string CreditHrs { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public int EnrolledStudents { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class InstructorCourseCard
    {
        public string OfferingId { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public int Batch { get; set; }
        public int CreditHours { get; set; }
        public string CourseType { get; set; } = string.Empty;
        public int StudentsCount { get; set; }
        public string BannerColorClass { get; set; } = "#0d6efd";
        public bool IsArchived { get; set; }
    }

    public class InstructorWeeklySummary
    {
        public int CurrentWeek { get; set; }
        public string Semester { get; set; } = string.Empty;
        public int QuizzesCompleted { get; set; }
        public int QuizzesPending { get; set; }
        public int AssignmentsCompleted { get; set; }
        public int AssignmentsPending { get; set; }
        public int MidsCompleted { get; set; }
        public int MidsPending { get; set; }
        public int FinalsCompleted { get; set; }
        public int FinalsPending { get; set; }
    }

    /*
    public class InstructorAttendanceStudentRow
    {
        public int SNo { get; set; }
        public string RollNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int EnrollId { get; set; } // Crucial for mapping to the DB
        public List<AttendanceEntry> Attendances { get; set; } = new List<AttendanceEntry>();
    }
    */

    public class SelectOption
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    public class StudentsViewModel
    {
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class InstructorPeopleViewModel
    {
        public string TeacherName { get; set; } = string.Empty;
        public string TeacherEmail { get; set; } = string.Empty;
        public List<StudentsViewModel> Students { get; set; } = new List<StudentsViewModel>();
        public StudentsViewModel? TeachingAssistant { get; set; }
    }
}
