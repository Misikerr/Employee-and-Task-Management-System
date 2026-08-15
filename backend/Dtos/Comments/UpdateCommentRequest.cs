using System.ComponentModel.DataAnnotations;

namespace EmployeeTaskManagement.Dtos.Comments
{
    public class UpdateCommentRequest
    {
        [Required(ErrorMessage = "Comment content is required.")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 2000 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
