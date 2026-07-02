using System;
using System.ComponentModel.DataAnnotations;

namespace OmniFlex.Models.ViewModels.Instructor
{
    public class CreateAssignmentViewModel
    {
        public string OfferingId { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string DeliveryMode { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }

        [Required]
        public decimal TotalMarks { get; set; }

        public decimal ActualWtg { get; set; }
        public string IsGraded { get; set; } = "Y";
        public string GradingGroup { get; set; } = string.Empty;
        public int? CountBestOf { get; set; }
    }
}
