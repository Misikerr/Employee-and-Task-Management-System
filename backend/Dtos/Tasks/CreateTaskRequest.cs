using System.ComponentModel.DataAnnotations;

namespace EmployeeTaskManagement.Dtos.Tasks
{
    public class CreateTaskRequest
    {
        [Required(ErrorMessage = "Task title is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Task title must be between 2 and 200 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Project ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Project ID must be a valid positive number.")]
        public int ProjectId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Assigned user ID must be a valid positive number.")]
        public int? AssignedToId { get; set; }

        public DateTime? DueDate { get; set; }

        [RegularExpression("^(Low|Medium|High|Critical)$", ErrorMessage = "Priority must be Low, Medium, High, or Critical.")]
        public string Priority { get; set; } = "Medium";

        [RegularExpression("^(Pending|InProgress|Completed|Cancelled)$", ErrorMessage = "Status must be Pending, InProgress, Completed, or Cancelled.")]
        public string Status { get; set; } = "Pending";
    }
}
