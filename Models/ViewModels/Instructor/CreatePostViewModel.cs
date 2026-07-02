using System.ComponentModel.DataAnnotations;

namespace OmniFlex.Models.ViewModels.Instructor
{
    public class CreatePostViewModel
    {
        public string OfferingId { get; set; } = string.Empty;

        [Required]
        public string PostType { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;
    }
}
