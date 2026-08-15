namespace EmployeeTaskManagement.Models
{
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime DueDate { get; set; }
        public int ProjectId { get; set; }
        public int? AssignedToId { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Project Project { get; set; } = null!;
        public User? AssignedTo { get; set; }
        public User CreatedBy { get; set; } = null!;
        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }
}
