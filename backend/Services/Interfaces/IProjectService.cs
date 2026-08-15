using EmployeeTaskManagement.Dtos.Projects;

namespace EmployeeTaskManagement.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProjectResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProjectResponse> CreateAsync(CreateProjectRequest request, int createdById, CancellationToken cancellationToken = default);
        Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
