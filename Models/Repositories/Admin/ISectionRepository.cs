using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.DTOs;

namespace OmniFlex.Models.Repositories.Admin
{
    public interface ISectionRepository
    {
        Task<Section?> GetByIdAsync(string sectionId);
        Task<IEnumerable<Section>> GetAllAsync();
        Task<IEnumerable<SectionsDto>> GetAllDetailedAsync();
        Task<IEnumerable<SectionsDto>> GetByCourseAsync(string courseId);
        Task<IEnumerable<SectionsDto>> GetByTeacherAsync(string teacherId);
        Task<IEnumerable<SectionsDto>> GetByDepartmentAsync(string deptId);
        Task<IEnumerable<SectionsDto>> GetByTaAsync(string taId);
        Task<IEnumerable<SectionsDto>> GetBySemesterAsync(string semesterId);
        Task<int> CreateAsync(Section section);
        Task<int> UpdateAsync(Section section);
        Task<int> DeleteAsync(string sectionId);                                         
        Task<int> AssignCRAsync(string sectionId, string studentId);                 
        Task<int> AssignTeacherAsync(string sectionId, string teacherId);
        Task<int> AddTaAsync(string sectionId, string taId);
        Task<int> RemoveTaAsync(string sectionId, string taId);
        Task<int> GetEnrolledCountAsync(string sectionId);
        Task<bool> ExistsAsync(string sectionId);
        Task<int> AssignInstructorToCourseSectionsAsync(string courseId, string instructorId);
        Task<IEnumerable<StudentEnrollmentDto>> GetEnrolledStudentsAsync(string sectionId);
    }
}