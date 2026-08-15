namespace EmployeeTaskManagement.Dtos.Auth
{
    public class AuthResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? JobTitle { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
