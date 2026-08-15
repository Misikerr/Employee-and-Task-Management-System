using EmployeeTaskManagement.Common;
using EmployeeTaskManagement.Dtos.Auth;
using EmployeeTaskManagement.Exceptions;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeTaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var user = await _authService.RegisterAsync(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password,
                request.PhoneNumber,
                cancellationToken);

            return Ok(new AuthResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString(),
                JobTitle = user.JobTitle,
                Token = await _authService.LoginAsync(request.Email, request.Password, cancellationToken)
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var token = await _authService.LoginAsync(request.Email, request.Password, cancellationToken);

            var user = await _authService.GetUserByEmailAsync(request.Email, cancellationToken);

            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new AuthResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString(),
                JobTitle = user.JobTitle,
                Token = token
            });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, cancellationToken);
            return Ok(new { message = "Password changed successfully." });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new
            {
                Id = userIdClaim,
                Email = email,
                Role = role
            });
        }
    }
}
