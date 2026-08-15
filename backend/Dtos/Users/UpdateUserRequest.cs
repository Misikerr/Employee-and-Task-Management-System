using System.ComponentModel.DataAnnotations;

namespace EmployeeTaskManagement.Dtos.Users
{
    public class UpdateUserRequest
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters.")]
        public string? FirstName { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 100 characters.")]
        public string? LastName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Department ID must be a valid positive number.")]
        public int? DepartmentId { get; set; }

        [RegularExpression("^(ADMIN|MANAGER|EMPLOYEE)$", ErrorMessage = "Role must be ADMIN, MANAGER, or EMPLOYEE.")]
        public string? Role { get; set; }

        [StringLength(100, ErrorMessage = "Job title cannot exceed 100 characters.")]
        public string? JobTitle { get; set; }

        public bool? IsActive { get; set; }
    }
}
