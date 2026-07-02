using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.ViewModels.Student;
namespace OmniFlex.Models.Repositories.Admin
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetByEnrollmentAsync(int enrollId);
        Task<IEnumerable<Attendance>> GetBySectionAndDateAsync(string sectionId, DateTime date);
        Task<AttendanceViewModel> GetStudentAttendanceAsync(string userId);
        Task<int> MarkAsync(Attendance attendance);
        Task<int> UpdateAsync(Attendance attendance);
        Task<decimal> GetAttendancePercentageAsync(int enrollId);
    }
}
