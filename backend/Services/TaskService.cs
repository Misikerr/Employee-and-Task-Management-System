using EmployeeTaskManagement.Common;
using EmployeeTaskManagement.Data;
using EmployeeTaskManagement.Dtos.Tasks;
using EmployeeTaskManagement.Exceptions;
using EmployeeTaskManagement.Models;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using TaskStatus = EmployeeTaskManagement.Models.TaskStatus;

namespace EmployeeTaskManagement.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskResponse>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
        {
            return await _context.TaskItems
                .AsNoTracking()
                .Include(t => t.Project)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Where(t => t.ProjectId == projectId)
                .Select(t => new TaskResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    CreatedById = t.CreatedById,
                    CreatedByName = $"{t.CreatedBy.FirstName} {t.CreatedBy.LastName}",
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    DueDate = t.DueDate,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    CommentCount = t.Comments.Count
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskResponse>> GetAssignedToUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.TaskItems
                .AsNoTracking()
                .Include(t => t.Project)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Where(t => t.AssignedToId == userId)
                .Select(t => new TaskResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    CreatedById = t.CreatedById,
                    CreatedByName = $"{t.CreatedBy.FirstName} {t.CreatedBy.LastName}",
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    DueDate = t.DueDate,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    CommentCount = t.Comments.Count
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<TaskResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.TaskItems
                .AsNoTracking()
                .Include(t => t.Project)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Where(t => t.Id == id)
                .Select(t => new TaskResponse
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project.Name,
                    CreatedById = t.CreatedById,
                    CreatedByName = $"{t.CreatedBy.FirstName} {t.CreatedBy.LastName}",
                    AssignedToId = t.AssignedToId,
                    AssignedToName = t.AssignedTo != null ? $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}" : null,
                    DueDate = t.DueDate,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    CommentCount = t.Comments.Count
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, int createdById, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ThrowIfNullOrWhitespace(request.Title, nameof(request.Title));

            var project = await _context.Projects
                .Include(p => p.CreatedBy)
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);

            if (project == null)
            {
                throw new ResourceNotFoundException("Project", request.ProjectId.ToString());
            }

            var creator = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == createdById, cancellationToken);

            if (creator == null)
            {
                throw new ResourceNotFoundException("User", createdById.ToString());
            }

            User? assignedTo = null;
            if (request.AssignedToId.HasValue)
            {
                assignedTo = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == request.AssignedToId.Value, cancellationToken);

                if (assignedTo == null)
                {
                    throw new ResourceNotFoundException("User", request.AssignedToId.Value.ToString());
                }
            }

            var taskItem = new TaskItem
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                ProjectId = request.ProjectId,
                CreatedById = createdById,
                AssignedToId = request.AssignedToId,
                DueDate = request.DueDate ?? DateTime.UtcNow.AddDays(7),
                Priority = Enum.TryParse<TaskPriority>(request.Priority, true, out var priority) ? priority : TaskPriority.Medium,
                Status = Enum.TryParse<TaskStatus>(request.Status, true, out var taskStatus) ? taskStatus : TaskStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync(cancellationToken);

            return new TaskResponse
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                ProjectId = taskItem.ProjectId,
                ProjectName = project.Name,
                CreatedById = taskItem.CreatedById,
                CreatedByName = $"{creator.FirstName} {creator.LastName}",
                AssignedToId = taskItem.AssignedToId,
                AssignedToName = assignedTo != null ? $"{assignedTo.FirstName} {assignedTo.LastName}" : null,
                DueDate = taskItem.DueDate,
                Priority = taskItem.Priority.ToString(),
                Status = taskItem.Status.ToString(),
                CommentCount = 0
            };
        }

        public async Task<TaskResponse> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
        {
            var taskItem = await _context.TaskItems
                .Include(t => t.Project)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

            if (taskItem == null)
            {
                throw new ResourceNotFoundException("Task", id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
                taskItem.Title = request.Title.Trim();

            if (request.Description != null)
                taskItem.Description = request.Description.Trim();

            if (request.AssignedToId.HasValue)
            {
                var assignedTo = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == request.AssignedToId.Value, cancellationToken);
                if (assignedTo == null && request.AssignedToId.Value > 0)
                {
                    throw new ResourceNotFoundException("User", request.AssignedToId.Value.ToString());
                }
                taskItem.AssignedToId = request.AssignedToId.Value > 0 ? request.AssignedToId.Value : null;
            }

            if (request.DueDate.HasValue)
                taskItem.DueDate = request.DueDate.Value;

            if (!string.IsNullOrWhiteSpace(request.Priority))
                taskItem.Priority = Enum.TryParse<TaskPriority>(request.Priority, true, out var priority) ? priority : taskItem.Priority;

            if (!string.IsNullOrWhiteSpace(request.Status))
                taskItem.Status = Enum.TryParse<TaskStatus>(request.Status, true, out var status) ? status : taskItem.Status;

            taskItem.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new TaskResponse
            {
                Id = taskItem.Id,
                Title = taskItem.Title,
                Description = taskItem.Description,
                ProjectId = taskItem.ProjectId,
                ProjectName = taskItem.Project.Name,
                CreatedById = taskItem.CreatedById,
                CreatedByName = $"{taskItem.CreatedBy.FirstName} {taskItem.CreatedBy.LastName}",
                AssignedToId = taskItem.AssignedToId,
                AssignedToName = taskItem.AssignedTo != null ? $"{taskItem.AssignedTo.FirstName} {taskItem.AssignedTo.LastName}" : null,
                DueDate = taskItem.DueDate,
                Priority = taskItem.Priority.ToString(),
                Status = taskItem.Status.ToString(),
                CommentCount = taskItem.Comments.Count
            };
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var taskItem = await _context.TaskItems
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

            if (taskItem == null)
            {
                throw new ResourceNotFoundException("Task", id.ToString());
            }

            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
