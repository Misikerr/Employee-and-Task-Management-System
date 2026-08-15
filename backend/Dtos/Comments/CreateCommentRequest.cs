using System.ComponentModel.DataAnnotations;

namespace EmployeeTaskManagement.Dtos.Comments
{
    public class CreateCommentRequest
    {
        [Required(ErrorMessage = "Comment content is required.")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 2000 characters.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Task ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Task ID must be a valid positive number.")]
        public int TaskId { get; set; }
    }
}
