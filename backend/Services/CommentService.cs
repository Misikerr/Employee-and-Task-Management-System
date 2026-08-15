using EmployeeTaskManagement.Common;
using EmployeeTaskManagement.Data;
using EmployeeTaskManagement.Dtos.Comments;
using EmployeeTaskManagement.Exceptions;
using EmployeeTaskManagement.Models;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManagement.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CommentResponse>> GetByTaskIdAsync(int taskId, CancellationToken cancellationToken = default)
        {
            return await _context.TaskComments
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.TaskItemId == taskId)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentResponse
                {
                    Id = c.Id,
                    Content = c.Content,
                    TaskId = c.TaskItemId,
                    UserId = c.UserId,
                    UserName = $"{c.User.FirstName} {c.User.LastName}",
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<CommentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TaskComments
                .AsNoTracking()
                .Include(c => c.User)
                .Where(c => c.Id == id)
                .Select(c => new CommentResponse
                {
                    Id = c.Id,
                    Content = c.Content,
                    TaskId = c.TaskItemId,
                    UserId = c.UserId,
                    UserName = $"{c.User.FirstName} {c.User.LastName}",
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<CommentResponse> CreateAsync(CreateCommentRequest request, int userId, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ThrowIfNullOrWhitespace(request.Content, nameof(request.Content));

            var taskItem = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

            if (taskItem == null)
            {
                throw new ResourceNotFoundException("Task", request.TaskId.ToString());
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                throw new ResourceNotFoundException("User", userId.ToString());
            }

            var comment = new TaskComment
            {
                Content = request.Content.Trim(),
                TaskItemId = request.TaskId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.TaskComments.Add(comment);
            await _context.SaveChangesAsync(cancellationToken);

            return new CommentResponse
            {
                Id = comment.Id,
                Content = comment.Content,
                TaskId = comment.TaskItemId,
                UserId = comment.UserId,
                UserName = $"{user.FirstName} {user.LastName}",
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<CommentResponse> UpdateAsync(int id, UpdateCommentRequest request, int userId, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ThrowIfNullOrWhitespace(request.Content, nameof(request.Content));

            var comment = await _context.TaskComments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (comment == null)
            {
                throw new ResourceNotFoundException("Comment", id.ToString());
            }

            if (comment.UserId != userId)
            {
                throw new UnauthorizedException("You can only edit your own comments.");
            }

            comment.Content = request.Content.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return new CommentResponse
            {
                Id = comment.Id,
                Content = comment.Content,
                TaskId = comment.TaskItemId,
                UserId = comment.UserId,
                UserName = $"{comment.User.FirstName} {comment.User.LastName}",
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
        {
            var comment = await _context.TaskComments
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (comment == null)
            {
                throw new ResourceNotFoundException("Comment", id.ToString());
            }

            if (comment.UserId != userId)
            {
                throw new UnauthorizedException("You can only delete your own comments.");
            }

            _context.TaskComments.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
