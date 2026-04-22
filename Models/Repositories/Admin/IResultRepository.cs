using OmniFlex.Models.Domain.Student;
using OmniFlex.Models.DTOs;
using OmniFlex.Models.ViewModels.Student;

namespace OmniFlex.Models.Repositories.Admin
{
    public interface IResultRepository
    {
        Task<Result?> GetByStudentAndSectionAsync(string studentId, string sectionId);
        Task<IEnumerable<Result>> GetByStudentAsync(string studentId);
        Task<TranscriptViewModel?> GetTranscriptHeaderData(string studentId);
        Task<TranscriptViewModel> GetDetailedTranscriptAsync(string studentId);
        Task<IEnumerable<Result>> GetBySectionAsync(string sectionId);
        Task<int> SaveResultAsync(Result result);
        Task<int> UpdateGradeAsync(string studentId, string sectionId, string grade, decimal percentage);
    }
}
