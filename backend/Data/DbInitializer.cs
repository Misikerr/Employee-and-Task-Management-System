using EmployeeTaskManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EmployeeTaskManagement.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            // Apply migrations automatically
            await context.Database.MigrateAsync();

            // Seed Department
            Department? defaultDept = null;
            if (!await context.Departments.AnyAsync())
            {
                defaultDept = new Department
                {
                    Name = "Engineering",
                    Description = "Core engineering and development department",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Departments.Add(defaultDept);
                await context.SaveChangesAsync();
            }
            else
            {
                defaultDept = await context.Departments.FirstOrDefaultAsync();
            }

            // Seed Users
            if (!await context.Users.AnyAsync())
            {
                var adminUser = new User
                {
                    FirstName = "System",
                    LastName = "Admin",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPass123!"),
                    PhoneNumber = "123-456-7890",
                    Role = Role.ADMIN,
                    JobTitle = "System Administrator",
                    IsActive = true,
                    DepartmentId = defaultDept?.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var managerUser = new User
                {
                    FirstName = "Project",
                    LastName = "Manager",
                    Email = "manager@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("ManagerPass123!"),
                    PhoneNumber = "234-567-8901",
                    Role = Role.MANAGER,
                    JobTitle = "Engineering Lead",
                    IsActive = true,
                    DepartmentId = defaultDept?.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var backendEmployee = new User
                {
                    FirstName = "Alex",
                    LastName = "Rivera",
                    Email = "backend@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("EmployeePass123!"),
                    PhoneNumber = "345-678-9012",
                    Role = Role.EMPLOYEE,
                    JobTitle = "Backend Developer",
                    IsActive = true,
                    DepartmentId = defaultDept?.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var frontendEmployee = new User
                {
                    FirstName = "Sarah",
                    LastName = "Chen",
                    Email = "frontend@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("EmployeePass123!"),
                    PhoneNumber = "456-789-0123",
                    Role = Role.EMPLOYEE,
                    JobTitle = "Frontend Developer",
                    IsActive = true,
                    DepartmentId = defaultDept?.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var mobileEmployee = new User
                {
                    FirstName = "David",
                    LastName = "Kim",
                    Email = "mobile@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("EmployeePass123!"),
                    PhoneNumber = "567-890-1234",
                    Role = Role.EMPLOYEE,
                    JobTitle = "Application Developer",
                    IsActive = true,
                    DepartmentId = defaultDept?.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Users.AddRange(adminUser, managerUser, backendEmployee, frontendEmployee, mobileEmployee);
                await context.SaveChangesAsync();
            }
        }
    }
}
