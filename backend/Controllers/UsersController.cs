using EmployeeTaskManagement.Authorization;
using EmployeeTaskManagement.Dtos.Users;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = Policies.ManagerOrAdmin)]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            return Ok(users);
        }

        [HttpGet("employees")]
        [Authorize(Policy = Policies.ManagerOrAdmin)]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetEmployees(CancellationToken cancellationToken)
        {
            var employees = await _userService.GetEmployeesAsync(cancellationToken);
            return Ok(employees);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var created = await _userService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var updated = await _userService.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _userService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
