using EmployeeTaskManagement.Common;
using EmployeeTaskManagement.Data;
using EmployeeTaskManagement.Exceptions;
using EmployeeTaskManagement.Models;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmployeeTaskManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ThrowIfNullOrWhitespace(email, nameof(email));
            ValidationHelper.ThrowIfNullOrWhitespace(password, nameof(password));

            var user = await GetUserByEmailAsync(email, cancellationToken);

            if (user == null || !user.IsActive || !VerifyPassword(password, user.PasswordHash))
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            return GenerateJwtToken(user);
        }

        public async Task<User> RegisterAsync(string firstName, string lastName, string email, string password, string? phoneNumber, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ThrowIfNullOrWhitespace(firstName, nameof(firstName));
            ValidationHelper.ThrowIfNullOrWhitespace(lastName, nameof(lastName));
            ValidationHelper.ThrowIfNullOrWhitespace(email, nameof(email));
            ValidationHelper.ThrowIfNullOrWhitespace(password, nameof(password));
            ValidationHelper.ThrowIfInvalid(password.Length >= 6, nameof(password), "Password must be at least 6 characters long.");

            var normalizedEmail = email.Trim();

            if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken))
            {
                throw new ConflictException("A user with this email already exists.");
            }

            var user = new User
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Email = normalizedEmail,
                PhoneNumber = phoneNumber,
                PasswordHash = HashPassword(password),
                Role = Role.EMPLOYEE,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return user;
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
            var issuer = _configuration["Jwt:Issuer"] ?? "EmployeeTaskManagement";
            var audience = _configuration["Jwt:Audience"] ?? "EmployeeTaskManagement";
            var expiresInMinutes = int.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var value) ? value : 60;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            ValidationHelper.ThrowIfNullOrWhitespace(currentPassword, nameof(currentPassword));
            ValidationHelper.ThrowIfNullOrWhitespace(newPassword, nameof(newPassword));
            ValidationHelper.ThrowIfInvalid(newPassword.Length >= 6, nameof(newPassword), "New password must be at least 6 characters long.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                throw new ResourceNotFoundException("User", userId.ToString());
            }

            if (!VerifyPassword(currentPassword, user.PasswordHash))
            {
                throw new UnauthorizedException("Incorrect current password.");
            }

            user.PasswordHash = HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
