using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;
using OmniFlex.Models.ViewModels.Student;
using OmniFlex.Models.DTOs;

namespace OmniFlex.Models.Repositories.Admin
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly DbConnectionFactory _factory;

        public EnrollmentRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            ENROLL_ID   AS EnrollId,
            STUDENT_ID  AS StudentId,
            ''          AS SectionId,
            OFFERING_ID AS OfferingId,
            ENROLL_DATE AS EnrollDate,
            STATUS      AS Status";

        public async Task<Enrollment?> GetByIdAsync(int enrollId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryFirstOrDefaultAsync<Enrollment>(
                $"SELECT {SELECT_COLUMNS} FROM ENROLLMENTS WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId });
        }

        public async Task<List<ClassCard>> GetEnrolledClassesAsync(string studentId)
        {
            const string sql = @"SELECT
                        C.COURSE_ID       AS CourseCode,
                        C.COURSE_NAME     AS CourseName,
                        T.FIRST_NAME || ' ' || T.LAST_NAME AS TeacherName,
                        S.SECTION_LABEL   AS Section,
                        S.BATCH AS Batch,
                        S.DEGREE AS Degree
                FROM ENROLLMENTS E
                JOIN SECTION_OFFERINGS SO ON SO.OFFERING_ID = E.OFFERING_ID
                JOIN SEMESTERS SM        ON SM.SEMESTER_ID = SO.SEMESTER_ID
                JOIN COURSES C           ON C.COURSE_ID    = SO.COURSE_ID
                JOIN SECTIONS S          ON SO.SECTION_ID  = S.SECTION_ID
                LEFT JOIN USERS T        ON T.USER_ID      = SO.TEACHER_ID
                WHERE E.STUDENT_ID = :StudentId AND E.STATUS = 'Registered' AND SM.IS_CURRENT = 1
                ORDER BY C.COURSE_NAME";


            using var conn = _factory.CreateConnection();

            return (await conn.QueryAsync<ClassCard>(sql, new { StudentId = studentId })).ToList();
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentAsync(string studentId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Enrollment>(
                $"SELECT {SELECT_COLUMNS} FROM ENROLLMENTS WHERE STUDENT_ID = :StudentId",
                new { StudentId = studentId });
        }
        public async Task<Enrollment?> GetRecordAsync(string studentId, string courseId)
        {
            using var conn = _factory.CreateConnection();

            // Student that's being appointed as TA must have completed the assigned course with requirements fulfilled
            string query = @"
                SELECT 
                    STUDENT_ID AS StudentId,
                    COURSE_ID  AS CourseId,
                    GRADE      AS Grade,
                    STATUS     AS Status
                FROM ENROLLMENTS 
                WHERE STUDENT_ID = :StudentId 
                AND COURSE_ID = :CourseId";

            return await conn.QueryFirstOrDefaultAsync<Enrollment>(query,
                new { StudentId = studentId, CourseId = courseId });
        }

        public async Task<IEnumerable<Enrollment>> GetBySectionAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.QueryAsync<Enrollment>(
                $@"SELECT {SELECT_COLUMNS} FROM ENROLLMENTS 
                   WHERE OFFERING_ID IN (
                       SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                   )",
                new { SectionId = sectionId });
        }

        /*public async Task<CoursePostDto> GetCoursePostsAsync(string offeringId)
        {
            var sql = @"
            -- Result Set 1: Posts
            SELECT 
                p.POST_ID,
                p.POST_TYPE AS PostType,
                p.TITLE AS Title,
                p.CONTENT AS Content, 
                p.CREATED_AT AS PostedTimeAndDate, 
                p.UPDATED_AT AS UpdatedTimeAndDate,
                u.FIRST_NAME || ' ' || u.LAST_NAME AS TeacherName
            FROM COURSE_POSTS p
            JOIN USERS u ON p.POSTED_BY = u.USER_ID
            WHERE p.OFFERING_ID = :OfferingId -- 'O02'
            ORDER BY p.CREATED_AT DESC;

            -- Result Set 2: Comments
            SELECT 
                c.POST_ID, -- Needed for mapping
                c.CONTENT AS Content,
                c.CREATED_AT AS CommentedTimeAndDate,
                u.FIRST_NAME || ' ' || u.LAST_NAME AS CommentorName
            FROM POST_COMMENTS c
            JOIN USERS u ON c.USER_ID = u.USER_ID
            WHERE c.POST_ID IN (SELECT POST_ID FROM COURSE_POSTS WHERE OFFERING_ID = :OfferingId)
            ORDER BY c.CREATED_AT ASC;

            -- Result Set 3: Attachments
            SELECT 
                pf.POST_ID, -- Needed for mapping
                f.FILE_NAME AS FileName,
                f.FILE_PATH AS FilePath
            FROM POST_FILES pf
            JOIN FILES f ON pf.FILE_ID = f.FILE_ID
            WHERE pf.POST_ID IN (SELECT POST_ID FROM COURSE_POSTS WHERE OFFERING_ID = :OfferingId);";

            using (var multi = await _factory.CreateConnection().QueryMultipleAsync(sql, new { OfferingId = offeringId }))
            {
                // 1. Get the Posts and keep the ID for mapping
                var postsData = (await multi.ReadAsync<dynamic>()).ToList();

                // 2. Map Comments and Attachments into Dictionaries for O(1) lookup speed
                var commentsLookup = (await multi.ReadAsync<dynamic>())
                    .GroupBy(c => (long)c.POST_ID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var attachmentsLookup = (await multi.ReadAsync<dynamic>())
                    .GroupBy(a => (long)a.POST_ID)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var finalPosts = new List<CoursePostModelDto>();

                foreach (var p in postsData)
                {
                    var postId = (long)p.POST_ID;
                    var postDto = new CoursePostModelDto
                    {
                        TeacherName = p.TeacherName,
                        Title = p.Title,
                        Content = p.Content,
                        PostType = p.PostType,
                        PostedTimeAndDate = p.PostedTimeAndDate,
                        UpdatedTimeAndDate = p.UpdatedTimeAndDate,
                        // Assign lists, defaulting to empty list if none found
                        Comments = commentsLookup.ContainsKey(postId)
                            ? commentsLookup[postId].Select(c => new CourseCommentDto
                            {
                                CommentorName = c.CommentorName,
                                Content = c.Content,
                                CommentedTimeAndDate = c.CommentedTimeAndDate
                            }).ToList()
                            : new List<CourseCommentDto>(),

                        Attachments = attachmentsLookup.ContainsKey(postId)
                            ? attachmentsLookup[postId].Select(a => new PostAttachmentDto
                            {
                                FileName = a.FileName,
                                FilePath = a.FilePath
                            }).ToList()
                            : new List<PostAttachmentDto>()
                    };
                    finalPosts.Add(postDto);
                }

                return new CoursePostDto { Posts = finalPosts };
            }
        }*/

        public async Task<CoursePostDto> GetCoursePostsAsync(string offeringId)
        {
            // Define the 3 separate queries (Note: No semicolons at the end for Oracle commands)
            var postsSql = @"
        SELECT 
            p.POST_ID AS PostId,
            p.POST_TYPE AS PostType,
            p.TITLE AS Title,
            p.CONTENT AS Content, 
            p.CREATED_AT AS PostedTimeAndDate, 
            p.UPDATED_AT AS UpdatedTimeAndDate,
            u.FIRST_NAME || ' ' || u.LAST_NAME AS TeacherName
        FROM COURSE_POSTS p
        JOIN USERS u ON p.POSTED_BY = u.USER_ID
        WHERE p.OFFERING_ID = :OfferingId
        ORDER BY p.CREATED_AT DESC";

            var commentsSql = @"
        SELECT 
            c.POST_ID AS PostId,
            c.CONTENT AS Content,
            c.CREATED_AT AS CommentedTimeAndDate,
            u.FIRST_NAME || ' ' || u.LAST_NAME AS CommentorName
        FROM POST_COMMENTS c
        JOIN USERS u ON c.USER_ID = u.USER_ID
        WHERE c.POST_ID IN (SELECT POST_ID FROM COURSE_POSTS WHERE OFFERING_ID = :OfferingId)
        ORDER BY c.CREATED_AT ASC";

            var attachmentsSql = @"
        SELECT 
            pf.POST_ID AS PostId,
            f.FILE_NAME AS FileName,
            f.FILE_PATH AS FilePath
        FROM POST_FILES pf
        JOIN FILES f ON pf.FILE_ID = f.FILE_ID
        WHERE pf.POST_ID IN (SELECT POST_ID FROM COURSE_POSTS WHERE OFFERING_ID = :OfferingId)";

            using (var conn = _factory.CreateConnection())
            {

                // 1. Execute all three queries in parallel or sequence
                var postsTask = conn.QueryAsync<CoursePostModelDto>(postsSql, new { OfferingId = offeringId });
                var commentsTask = conn.QueryAsync<CourseCommentDto>(commentsSql, new { OfferingId = offeringId });
                var attachmentsTask = conn.QueryAsync<PostAttachmentDto>(attachmentsSql, new { OfferingId = offeringId });

                // Wait for all data to return
                await Task.WhenAll(postsTask, commentsTask, attachmentsTask);

                var postsData = postsTask.Result;

                // 2. Group Comments & Attachments by POST_ID for easy lookups
                var commentsLookup = commentsTask.Result
                    .GroupBy(c => c.PostId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var attachmentsLookup = attachmentsTask.Result
                    .GroupBy(a => a.PostId)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // 3. Map into DTOs
                //var finalPosts = new List<CoursePostModelDto>();

                var finalPosts = postsData.Select(p =>
                {
                    p.Comments = commentsLookup.ContainsKey(p.PostId)
                    ? commentsLookup[p.PostId]
                    : new List<CourseCommentDto>();

                    p.Attachments = attachmentsLookup.ContainsKey(p.PostId)
                    ? attachmentsLookup[p.PostId]
                    : new List<PostAttachmentDto>();

                    return p;
                }).ToList();

                return new CoursePostDto { Posts = finalPosts };
            }
        }

        public async Task<string> GetOfferingIdByCourseCodeAsync(string courseId, string studentId)
        {
            var sql = @"
                SELECT SO.OFFERING_ID
                FROM ENROLLMENTS E
                JOIN SECTION_OFFERINGS SO ON SO.OFFERING_ID = E.OFFERING_ID
                WHERE E.STUDENT_ID = :StudentId AND SO.COURSE_ID = :CourseId AND E.STATUS = 'Registered'";
            return await _factory.CreateConnection().ExecuteScalarAsync<string?>(sql, new { StudentId = studentId, CourseId = courseId }) ?? string.Empty;
        }

        public async Task<int> AddPostCommentAsync(long postId, string userId, string content)
        {
            using var conn = _factory.CreateConnection();

            var sql = @"
                INSERT INTO POST_COMMENTS (POST_ID, USER_ID, CONTENT, CREATED_AT)
                VALUES (:PostId, :UserId, :Content, SYSTIMESTAMP)";
            return await conn.ExecuteAsync(sql, new { PostId = postId, UserId = userId, Content = content });
        }

        public async Task<int> EnrollAsync(Enrollment enrollment)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(@"
                INSERT INTO ENROLLMENTS (STUDENT_ID, OFFERING_ID, ENROLL_DATE, STATUS)
                VALUES (:StudentId, :OfferingId, :EnrollDate, :Status)", enrollment);
        }

        public async Task<int> TransferAsync(int enrollId, string newSectionId)
        {
            using var conn = _factory.CreateConnection();
            
            // Get the OFFERING_ID for the new section
            var newOfferingId = await conn.ExecuteScalarAsync<string>(
                "SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId",
                new { SectionId = newSectionId });
            
            return await conn.ExecuteAsync(
                "UPDATE ENROLLMENTS SET OFFERING_ID = :NewOfferingId WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId, NewOfferingId = newOfferingId });
        }

        public async Task<int> UpdateStatusAsync(int enrollId, string status)
        {
            using var conn = _factory.CreateConnection();
            
            return await conn.ExecuteAsync(
                "UPDATE ENROLLMENTS SET STATUS = :Status WHERE ENROLL_ID = :EnrollId",
                new { EnrollId = enrollId, Status = status });
        }

        public async Task<bool> IsEnrolledAsync(string studentId, string sectionId)
        {
            using var conn = _factory.CreateConnection();
            
            var count = await conn.ExecuteScalarAsync<int>(
                @"SELECT COUNT(*) FROM ENROLLMENTS 
                  WHERE STUDENT_ID = :StudentId AND OFFERING_ID IN (
                      SELECT OFFERING_ID FROM SECTION_OFFERINGS WHERE SECTION_ID = :SectionId
                  )",
                new { StudentId = studentId, SectionId = sectionId });
            return count > 0;
        }
    }
}
