using System.ComponentModel.DataAnnotations;

namespace EmployeeTaskManagement.Dtos.Projects
{
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "Project name is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Project name must be between 2 and 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [RegularExpression("^(Planning|Active|Completed|Cancelled)$", ErrorMessage = "Status must be Planning, Active, Completed, or Cancelled.")]
        public string Status { get; set; } = "Planning";
    }
}
