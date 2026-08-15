namespace EmployeeTaskManagement.Middleware
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? ErrorCode { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, List<string>>? ValidationErrors { get; set; }
    }
}
