using OmniFlex.Models.DTOs;
using OmniFlex.Models.Domain.Instructor;
using OmniFlex.Models.ViewModels.Instructor;

namespace OmniFlex.Models.Repositories.Instructor
{
    public interface IInstructorRepository
    {
        Task<IEnumerable<AssignedClassesDTO>> GetAssignedClassesAsync(string instructorId);
        Task<InstructorDashboardViewModel> GetDashboardAsync(string instructorId);
        Task<List<InstructorCourseCard>> GetCoursesAsync(string instructorId);
        Task<InstructorClassroomViewModel> GetInstructorClassroomAsync(string offeringId);
        Task<InstructorPeopleViewModel> GetInstructorPeopleAsync(string offeringId);
        Task<InstructorAssignmentViewModel> GetInstructorAssignmentDetailsAsync(int assignmentId);
        Task<GradesViewModel> GetInstructorGradesGridAsync(string offeringId);
        Task<int> GetEnrollmentIDAsync(string studentId, string offeringId);
        Task<OnsiteExamViewModel> GetOnsiteAssignmentDetailsAsync(string offeringId, int assignmentId);
        Task<IEnumerable<Exam>> BulkUpsertGradesAsync(List<Exam> entries);
        Task<int> AddCoursePostAsync(CoursePost post);
        Task<int> AddAssignmentAsync(Assignment assignment);
        Task<int> GradeAssignmentPhysicalAsync(int assignmentId, int enrollmentId, string teacherId, decimal marksObtained, string remarks, DateTime ExamDate);
        //Task<InstructorWeeklyCalendarViewModel> GetWeeklyCalendarAsync(string instructorId);
        //Task<InstructorAttendanceViewModel> GetManageAttendanceModelAsync(string instructorId, string? selectedCourseId = null, string? selectedSectionId = null, string? selectedMonth = null);
    }
}
