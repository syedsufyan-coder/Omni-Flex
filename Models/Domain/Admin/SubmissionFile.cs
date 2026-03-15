namespace OmniFlex.Models.Domain.Admin
{
    public class SubmissionFile
    {
        public int SubFileId { get; set; }
        public string SubmissionId { get; set; } = string.Empty;
        public int FileId { get; set; }
    }
}
