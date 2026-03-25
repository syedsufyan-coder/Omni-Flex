using OmniFlex.Models.Domain.Student;

namespace OmniFlex.Models.Repositories.Admin
{
    public interface IResultRepository
    {
        Task<Result?> GetByStudentAndSectionAsync(string studentId, string sectionId);
        Task<IEnumerable<Result>> GetByStudentAsync(string studentId);
        Task<IEnumerable<Result>> GetBySectionAsync(string sectionId);
        Task<int> SaveResultAsync(Result result);
        Task<int> UpdateGradeAsync(string studentId, string sectionId, string grade, decimal percentage);
    }
}
