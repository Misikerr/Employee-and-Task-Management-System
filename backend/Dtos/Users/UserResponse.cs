namespace EmployeeTaskManagement.Dtos.Users
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public bool IsActive { get; set; }
    }
}
