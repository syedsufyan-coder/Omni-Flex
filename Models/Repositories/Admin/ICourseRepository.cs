using OmniFlex.Models.DTOs;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public interface ICourseRepository
    {
        Task<Course?> GetByIdAsync(string courseId);
        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<CourseDto>> GetByDeptWithDetailsAsync(string deptId);
        Task<IEnumerable<CourseDto>> GetByTeacherWithDetailsAsync(string teacherId);
        Task<IEnumerable<CourseDto>> GetBySectionWithDetailsAsync(string sectionId);
        Task<IEnumerable<CourseDto>> GetByCreditsWithDetailsAsync(int creditHrs);
        Task<IEnumerable<CourseDto>> GetByCourseTypeWithDetailsAsync(string courseType);
        Task<IEnumerable<CourseDto>> GetByPreRequisiteWithDetailsAsync(string preRequisiteId);
        Task<IEnumerable<CourseDto>> GetByCourseCatWithDetailsAsync(string courseCat);
        Task<IEnumerable<Course>> GetActiveAsync();
        Task<IEnumerable<Course>> GetByDeptAsync(string deptId);
        Task<int> CreateAsync(Course course);
        Task<int> UpdateAsync(Course course);
        Task<int> DeleteAsync(string courseId);
        Task<int> SetActiveStatusAsync(string courseId, int status);
    }
}
