using OmniFlex.Models.DTOs;
using OmniFlex.Models.ViewModels.Instructor;

namespace OmniFlex.Models.Repositories.Instructor
{
    public interface IInstructorRepository
    {
        Task<IEnumerable<AssignedClassesDTO>> GetAssignedClassesAsync(string instructorId);
        Task<InstructorDashboardViewModel> GetDashboardAsync(string instructorId);
        Task<List<InstructorCourseCard>> GetCoursesAsync(string instructorId);
        //Task<InstructorWeeklyCalendarViewModel> GetWeeklyCalendarAsync(string instructorId);
        //Task<InstructorAttendanceViewModel> GetManageAttendanceModelAsync(string instructorId, string? selectedCourseId = null, string? selectedSectionId = null, string? selectedMonth = null);
    }
}
