using System.ComponentModel.DataAnnotations;

namespace EmployeeTaskManagement.Dtos.Auth
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
