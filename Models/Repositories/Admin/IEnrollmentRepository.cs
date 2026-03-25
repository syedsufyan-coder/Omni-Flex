using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.ViewModels.Student;

namespace OmniFlex.Models.Repositories.Admin
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetByIdAsync(int enrollId);
        Task<List<ClassCard>> GetEnrolledClassesAsync(string studentId);
        Task<IEnumerable<Enrollment>> GetByStudentAsync(string studentId);
        Task<IEnumerable<Enrollment>> GetBySectionAsync(string sectionId);
        Task<int> EnrollAsync(Enrollment enrollment);
        Task<int> TransferAsync(int enrollId, string newSectionId);
        Task<int> UpdateStatusAsync(int enrollId, string status);
        Task<bool> IsEnrolledAsync(string studentId, string sectionId);
    }
}
