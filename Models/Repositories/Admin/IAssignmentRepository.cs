using OmniFlex.Models.Domain.Student;

namespace OmniFlex.Models.Repositories.Admin
{
    public interface IAssignmentRepository
    {
        Task<Assignment?> GetByIdAsync(string assignmentId);
        Task<IEnumerable<Assignment>> GetBySectionAsync(string sectionId);
        Task<int> CreateAsync(Assignment assignment);
        Task<int> UpdateAsync(Assignment assignment);
        Task<int> DeleteAsync(string assignmentId);
    }
}
