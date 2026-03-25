using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.DTOs;
using OmniFlex.Models.ViewModels.Student;
namespace OmniFlex.Models.Repositories.Admin
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string userId);
        Task<StudentDto?> GetDetailsByIdAsync(string userId);
        Task<List<EnrolledCourseRow>> GetEnrolledCoursesAsync(string userId);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetByRoleAsync(string role);
        Task<int> CreateAsync(User user);
        Task<int> UpdateAsync(User user);
        Task<int> DeleteAsync(string userId);
        Task<bool> ExistsAsync(string userId);
        Task<int> GetCountByRoleAsync(string role);
    }
}
