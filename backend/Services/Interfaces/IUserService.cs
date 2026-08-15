using EmployeeTaskManagement.Dtos.Users;

namespace EmployeeTaskManagement.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<UserResponse>> GetEmployeesAsync(CancellationToken cancellationToken = default);
        Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
        Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
