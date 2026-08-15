using EmployeeTaskManagement.Models;

namespace EmployeeTaskManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<User> RegisterAsync(string firstName, string lastName, string email, string password, string? phoneNumber, CancellationToken cancellationToken = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    }
}
