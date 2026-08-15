using EmployeeTaskManagement.Dtos.Comments;

namespace EmployeeTaskManagement.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentResponse>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken = default);
        Task<CommentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<CommentResponse> CreateAsync(CreateCommentRequest request, int userId, CancellationToken cancellationToken = default);
        Task<CommentResponse> UpdateAsync(int id, UpdateCommentRequest request, int userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, int userId, CancellationToken cancellationToken = default);
    }
}
