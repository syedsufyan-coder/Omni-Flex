using Dapper;
using OmniFlex.Infrastructure;
using OmniFlex.Models.Domain.Admin;

namespace OmniFlex.Models.Repositories.Admin
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly DbConnectionFactory _factory;

        public AnnouncementRepository(DbConnectionFactory factory)
            => _factory = factory;

        private const string SELECT_COLUMNS = @"
            ANNOUNCEMENT_ID AS AnnouncementId,
            POSTED_BY       AS PostedBy,
            SECTION_ID      AS SectionId,
            TITLE           AS Title,
            CONTENT         AS Content,
            AUDIENCE        AS Audience,
            IS_PINNED       AS IsPinned,
            PRIORITY        AS Priority,
            POST_DATE       AS PostDate";

        public async Task<IEnumerable<Announcement>> GetAllSystemWideAsync()
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Announcement>(
                $"SELECT {SELECT_COLUMNS} FROM ANNOUNCEMENTS WHERE SECTION_ID IS NULL ORDER BY POST_DATE DESC");
        }

        public async Task<IEnumerable<Announcement>> GetBySectionAsync(string sectionId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.QueryAsync<Announcement>(
                $"SELECT {SELECT_COLUMNS} FROM ANNOUNCEMENTS WHERE SECTION_ID = :SectionId ORDER BY POST_DATE DESC",
                new { SectionId = sectionId });
        }

        public async Task<int> CreateAsync(Announcement announcement)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(@"
                INSERT INTO ANNOUNCEMENTS 
                (ANNOUNCEMENT_ID, POSTED_BY, SECTION_ID, TITLE, CONTENT, 
                 AUDIENCE, IS_PINNED, PRIORITY, POST_DATE)
                VALUES 
                (:AnnouncementId, :PostedBy, :SectionId, :Title, :Content,
                 :Audience, :IsPinned, :Priority, :PostDate)", announcement);
        }

        public async Task<int> DeleteAsync(string announcementId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(
                "DELETE FROM ANNOUNCEMENTS WHERE ANNOUNCEMENT_ID = :AnnouncementId",
                new { AnnouncementId = announcementId });
        }

        public async Task<int> TogglePinAsync(string announcementId)
        {
            using var conn = _factory.CreateConnection();
            conn.Open();
            return await conn.ExecuteAsync(
                "UPDATE ANNOUNCEMENTS SET IS_PINNED = CASE WHEN IS_PINNED = 1 THEN 0 ELSE 1 END WHERE ANNOUNCEMENT_ID = :AnnouncementId",
                new { AnnouncementId = announcementId });
        }
    }
}
