namespace EmployeeTaskManagement.Models
{
    public class TaskComment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int TaskItemId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public TaskItem TaskItem { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
