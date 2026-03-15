using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public interface ISubmissionRepository
    {
        Task<Submission?> GetByIdAsync(string submissionId);
        Task<IEnumerable<Submission>> GetByAssignmentAsync(string assignmentId);
        Task<IEnumerable<Submission>> GetByStudentAsync(string studentId);
        Task<int> CreateAsync(Submission submission);
        Task<int> GradeAsync(string submissionId, decimal marks, decimal wtg, string gradedBy);
        Task<int> LockAsync(string submissionId);
    }
}
