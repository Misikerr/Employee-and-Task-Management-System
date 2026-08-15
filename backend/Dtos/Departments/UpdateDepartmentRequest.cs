using System.ComponentModel.DataAnnotations;

namespace EmployeeTaskManagement.Dtos.Departments
{
    public class UpdateDepartmentRequest
    {
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Department name must be between 2 and 150 characters.")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
