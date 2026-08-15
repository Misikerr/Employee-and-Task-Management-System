using EmployeeTaskManagement.Dtos.Tasks;

namespace EmployeeTaskManagement.Services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskResponse>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskResponse>> GetAssignedToUserAsync(int userId, CancellationToken cancellationToken = default);
        Task<TaskResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TaskResponse> CreateAsync(CreateTaskRequest request, int createdById, CancellationToken cancellationToken = default);
        Task<TaskResponse> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
