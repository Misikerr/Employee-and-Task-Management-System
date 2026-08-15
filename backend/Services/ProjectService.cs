using EmployeeTaskManagement.Common;
using EmployeeTaskManagement.Data;
using EmployeeTaskManagement.Dtos.Projects;
using EmployeeTaskManagement.Exceptions;
using EmployeeTaskManagement.Models;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManagement.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.CreatedBy)
                .Select(p => new ProjectResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Status = p.Status.ToString(),
                    CreatedById = p.CreatedById,
                    CreatedByName = $"{p.CreatedBy.FirstName} {p.CreatedBy.LastName}",
                    TaskCount = p.Tasks.Count
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ProjectResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Projects
                .AsNoTracking()
                .Include(p => p.CreatedBy)
                .Where(p => p.Id == id)
                .Select(p => new ProjectResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Status = p.Status.ToString(),
                    CreatedById = p.CreatedById,
                    CreatedByName = $"{p.CreatedBy.FirstName} {p.CreatedBy.LastName}",
                    TaskCount = p.Tasks.Count
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request, int createdById, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ThrowIfNullOrWhitespace(request.Name, nameof(request.Name));
            var normalizedName = request.Name.Trim();

            if (await _context.Projects.AnyAsync(p => p.Name == normalizedName, cancellationToken))
            {
                throw new ConflictException("A project with this name already exists.");
            }

            var creator = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == createdById, cancellationToken);

            if (creator == null)
            {
                throw new ResourceNotFoundException("User", createdById.ToString());
            }

            var project = new Project
            {
                Name = normalizedName,
                Description = request.Description?.Trim(),
                Status = Enum.TryParse<ProjectStatus>(request.Status, true, out var status) ? status : ProjectStatus.Planning,
                CreatedById = createdById,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync(cancellationToken);

            return new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                CreatedById = project.CreatedById,
                CreatedByName = $"{creator.FirstName} {creator.LastName}",
                TaskCount = 0
            };
        }

        public async Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request, CancellationToken cancellationToken = default)
        {
            var project = await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (project == null)
            {
                throw new ResourceNotFoundException("Project", id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var normalizedName = request.Name.Trim();
                if (await _context.Projects.AnyAsync(p => p.Name == normalizedName && p.Id != id, cancellationToken))
                {
                    throw new ConflictException("A project with this name already exists.");
                }
                project.Name = normalizedName;
            }

            if (request.Description != null)
                project.Description = request.Description.Trim();

            if (!string.IsNullOrWhiteSpace(request.Status))
                project.Status = Enum.TryParse<ProjectStatus>(request.Status, true, out var status) ? status : project.Status;

            project.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status.ToString(),
                CreatedById = project.CreatedById,
                CreatedByName = $"{project.CreatedBy.FirstName} {project.CreatedBy.LastName}",
                TaskCount = project.Tasks.Count
            };
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (project == null)
            {
                throw new ResourceNotFoundException("Project", id.ToString());
            }

            if (project.Tasks.Count > 0)
            {
                throw new ConflictException("Cannot delete project with active tasks. Delete or reassign tasks first.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
