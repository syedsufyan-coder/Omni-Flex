using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly DbConnectionFactory _factory;

        public SubmissionRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            SUBMISSION_ID  AS SubmissionId,
            ASSIGNMENT_ID  AS AssignmentId,
            STUDENT_ID     AS StudentId,
            SUBMIT_DATE    AS SubmitDate,
            OBTAINED_MARKS AS ObtainedMarks,
            OBTAINED_WTG   AS ObtainedWtg,
            IS_LATE        AS IsLate,
            GRADED_BY      AS GradedBy,
            GRADED_AT      AS GradedAt,
            LOCKED         AS Locked";

        public async Task<Submission?> GetByIdAsync(string submissionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryFirstOrDefaultAsync<Submission>(
                $"SELECT {SELECT_COLUMNS} FROM SUBMISSIONS WHERE SUBMISSION_ID = :SubmissionId",
                new { SubmissionId = submissionId });
        }

        public async Task<IEnumerable<Submission>> GetByAssignmentAsync(string assignmentId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Submission>(
                $"SELECT {SELECT_COLUMNS} FROM SUBMISSIONS WHERE ASSIGNMENT_ID = :AssignmentId",
                new { AssignmentId = assignmentId });
        }

        public async Task<IEnumerable<Submission>> GetByStudentAsync(string studentId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Submission>(
                $"SELECT {SELECT_COLUMNS} FROM SUBMISSIONS WHERE STUDENT_ID = :StudentId",
                new { StudentId = studentId });
        }

        public async Task<int> CreateAsync(Submission submission)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                INSERT INTO SUBMISSIONS 
                (SUBMISSION_ID, ASSIGNMENT_ID, STUDENT_ID, SUBMIT_DATE, OBTAINED_MARKS,
                 OBTAINED_WTG, IS_LATE, GRADED_BY, GRADED_AT, LOCKED)
                VALUES 
                (:SubmissionId, :AssignmentId, :StudentId, :SubmitDate, :ObtainedMarks,
                 :ObtainedWtg, :IsLate, :GradedBy, :GradedAt, :Locked)", submission);
        }

        public async Task<int> GradeAsync(string submissionId, decimal marks, decimal wtg, string gradedBy)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                UPDATE SUBMISSIONS SET 
                OBTAINED_MARKS = :Marks, OBTAINED_WTG = :Wtg, GRADED_BY = :GradedBy, GRADED_AT = SYSDATE
                WHERE SUBMISSION_ID = :SubmissionId",
                new { SubmissionId = submissionId, Marks = marks, Wtg = wtg, GradedBy = gradedBy });
        }

        public async Task<int> LockAsync(string submissionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(
                "UPDATE SUBMISSIONS SET LOCKED = 1 WHERE SUBMISSION_ID = :SubmissionId",
                new { SubmissionId = submissionId });
        }
    }
}
