namespace EmployeeTaskManagement.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public int? DepartmentId { get; set; }
        public bool IsActive { get; set; } = true;
        public Role Role { get; set; } = Role.EMPLOYEE;
        public string? JobTitle { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Department? Department { get; set; }
        public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
        public ICollection<TaskItem> CreatedTasks { get; set; } = new List<TaskItem>();
        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }
}
