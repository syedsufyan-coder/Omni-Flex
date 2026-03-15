namespace OmniFlex.Models.Domain.Admin
{
    public class SubmissionComment
    {
        public int CommentId { get; set; }
        public string SubmissionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
