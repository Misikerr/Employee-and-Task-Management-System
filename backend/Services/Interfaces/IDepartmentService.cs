using EmployeeTaskManagement.Dtos.Departments;

namespace EmployeeTaskManagement.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<DepartmentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default);
        Task<DepartmentResponse> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
