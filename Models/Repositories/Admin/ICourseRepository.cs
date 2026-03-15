using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(string courseId);
        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<Course>> GetActiveAsync();
        Task<IEnumerable<Course>> GetByDeptAsync(string deptId);
        Task<int> CreateAsync(Course course);
        Task<int> UpdateAsync(Course course);
        Task<int> DeleteAsync(string courseId);
        Task<int> SetActiveStatusAsync(string courseId, int status);
    }
}
