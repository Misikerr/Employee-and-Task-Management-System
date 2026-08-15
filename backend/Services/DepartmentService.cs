using EmployeeTaskManagement.Common;
using EmployeeTaskManagement.Data;
using EmployeeTaskManagement.Dtos.Departments;
using EmployeeTaskManagement.Exceptions;
using EmployeeTaskManagement.Models;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManagement.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Departments
                .AsNoTracking()
                .Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    UserCount = d.Users.Count
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<DepartmentResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Departments
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new DepartmentResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    UserCount = d.Users.Count
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<DepartmentResponse> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ThrowIfNullOrWhitespace(request.Name, nameof(request.Name));

            var normalizedName = request.Name.Trim();

            if (await _context.Departments.AnyAsync(d => d.Name == normalizedName, cancellationToken))
            {
                throw new ConflictException("A department with this name already exists.");
            }

            var department = new Department
            {
                Name = normalizedName,
                Description = request.Description?.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync(cancellationToken);

            return new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                UserCount = 0
            };
        }

        public async Task<DepartmentResponse> UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
        {
            var department = await _context.Departments
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (department == null)
            {
                throw new ResourceNotFoundException("Department", id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var normalizedName = request.Name.Trim();
                if (await _context.Departments.AnyAsync(d => d.Name == normalizedName && d.Id != id, cancellationToken))
                {
                    throw new ConflictException("A department with this name already exists.");
                }
                department.Name = normalizedName;
            }

            if (request.Description != null)
                department.Description = request.Description.Trim();

            department.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new DepartmentResponse
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                UserCount = department.Users.Count
            };
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var department = await _context.Departments
                .Include(d => d.Users)
                .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

            if (department == null)
            {
                throw new ResourceNotFoundException("Department", id.ToString());
            }

            if (department.Users.Count > 0)
            {
                throw new ConflictException("Cannot delete department with active users. Reassign or remove users first.");
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
