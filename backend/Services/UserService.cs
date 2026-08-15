using EmployeeTaskManagement.Common;
using EmployeeTaskManagement.Data;
using EmployeeTaskManagement.Dtos.Users;
using EmployeeTaskManagement.Exceptions;
using EmployeeTaskManagement.Models;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeTaskManagement.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .GroupJoin(_context.Departments,
                    u => u.DepartmentId,
                    d => d.Id,
                    (u, depts) => new { u, depts })
                .SelectMany(
                    x => x.depts.DefaultIfEmpty(),
                    (x, d) => new UserResponse
                    {
                        Id = x.u.Id,
                        FirstName = x.u.FirstName,
                        LastName = x.u.LastName,
                        Email = x.u.Email,
                        PhoneNumber = x.u.PhoneNumber,
                        DepartmentId = x.u.DepartmentId,
                        DepartmentName = d != null ? d.Name : null,
                        Role = x.u.Role.ToString(),
                        JobTitle = x.u.JobTitle,
                        IsActive = x.u.IsActive
                    })
                .OrderBy(u => u.FirstName)
                .ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<UserResponse>> GetEmployeesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Role == Role.EMPLOYEE && u.IsActive)
                .GroupJoin(_context.Departments,
                    u => u.DepartmentId,
                    d => d.Id,
                    (u, depts) => new { u, depts })
                .SelectMany(
                    x => x.depts.DefaultIfEmpty(),
                    (x, d) => new UserResponse
                    {
                        Id = x.u.Id,
                        FirstName = x.u.FirstName,
                        LastName = x.u.LastName,
                        Email = x.u.Email,
                        PhoneNumber = x.u.PhoneNumber,
                        DepartmentId = x.u.DepartmentId,
                        DepartmentName = d != null ? d.Name : null,
                        Role = x.u.Role.ToString(),
                        JobTitle = x.u.JobTitle,
                        IsActive = x.u.IsActive
                    })
                .OrderBy(u => u.FirstName)
                .ToListAsync(cancellationToken);
        }


        public async Task<UserResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    DepartmentId = u.DepartmentId,
                    DepartmentName = u.Department != null ? u.Department.Name : null,
                    Role = u.Role.ToString(),
                    JobTitle = u.JobTitle,
                    IsActive = u.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ThrowIfNullOrWhitespace(request.FirstName, nameof(request.FirstName));
            ValidationHelper.ThrowIfNullOrWhitespace(request.LastName, nameof(request.LastName));
            ValidationHelper.ThrowIfNullOrWhitespace(request.Email, nameof(request.Email));
            ValidationHelper.ThrowIfNullOrWhitespace(request.Password, nameof(request.Password));
            ValidationHelper.ThrowIfInvalid(request.Password.Length >= 6, nameof(request.Password), "Password must be at least 6 characters long.");

            var normalizedEmail = request.Email.Trim();

            if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken))
            {
                throw new ConflictException("A user with this email already exists.");
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                DepartmentId = request.DepartmentId,
                Role = Enum.TryParse<Role>(request.Role, true, out var role) ? role : Role.EMPLOYEE,
                JobTitle = request.JobTitle?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DepartmentId = user.DepartmentId,
                Role = user.Role.ToString(),
                JobTitle = user.JobTitle,
                IsActive = user.IsActive
            };
        }

        public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
            {
                throw new ResourceNotFoundException("User", id.ToString());
            }

            if (!string.IsNullOrWhiteSpace(request.FirstName))
                user.FirstName = request.FirstName.Trim();

            if (!string.IsNullOrWhiteSpace(request.LastName))
                user.LastName = request.LastName.Trim();

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber.Trim();

            if (request.DepartmentId.HasValue)
                user.DepartmentId = request.DepartmentId.Value;

            if (!string.IsNullOrWhiteSpace(request.Role))
                user.Role = Enum.TryParse<Role>(request.Role, true, out var role) ? role : user.Role;

            if (request.JobTitle != null)
                user.JobTitle = request.JobTitle.Trim();

            if (request.IsActive.HasValue)
                user.IsActive = request.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DepartmentId = user.DepartmentId,
                Role = user.Role.ToString(),
                JobTitle = user.JobTitle,
                IsActive = user.IsActive
            };
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (user == null)
            {
                throw new ResourceNotFoundException("User", id.ToString());
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
