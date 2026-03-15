namespace OmniFlex.Models.Domain.Admin
{
    public class PostComment
    {
        public int CommentId { get; set; }
        public string PostId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int? ParentCommentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Visibility { get; set; } = "Public";
        public DateTime CreatedAt { get; set; }
    }
}
