using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public interface ISectionRepository
    {
        Task<Section?> GetByIdAsync(string sectionId);
        Task<IEnumerable<Section>> GetAllAsync();
        Task<IEnumerable<Section>> GetByCourseAsync(string courseId);
        Task<IEnumerable<Section>> GetByTeacherAsync(string teacherId);
        Task<IEnumerable<Section>> GetBySemesterAsync(string semesterId);
        Task<int> CreateAsync(Section section);
        Task<int> UpdateAsync(Section section);
        Task<int> AssignTeacherAsync(string sectionId, string teacherId);
        Task<int> AddTaAsync(string sectionId, string taId);
        Task<int> RemoveTaAsync(string sectionId, string taId);
        Task<int> GetEnrolledCountAsync(string sectionId);
    }
}
