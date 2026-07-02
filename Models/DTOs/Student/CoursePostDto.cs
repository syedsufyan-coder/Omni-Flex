using System;

namespace OmniFlex.Models.DTOs
{
    public class CourseCommentDto
    {
        public long PostId { get; set; } 
        public string CommentorName { get; set; }  = string.Empty;
        public DateTime CommentedTimeAndDate { get; set; }
        public DateTime? UpdatedTimeAndDate { get; set; }
        public string Content { get; set; } = string.Empty;
    }
    public class PostAttachmentDto
    {
        public long PostId { get; set; } 
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }

    public class CoursePostModelDto
    {
        public long PostId { get; set; } 
        public string TeacherName { get; set; } = string.Empty;
        public DateTime PostedTimeAndDate { get; set; }
        public string Content { get; set; } = string.Empty;
        public string PostType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime? UpdatedTimeAndDate { get; set; }
        public List<CourseCommentDto> Comments { get; set; } = new List<CourseCommentDto>();
        public List<PostAttachmentDto> Attachments { get; set; } = new List<PostAttachmentDto>();
    }

    public class CoursePostDto
    {
        public List<CoursePostModelDto> Posts { get; set; } = new List<CoursePostModelDto>();
    }

}