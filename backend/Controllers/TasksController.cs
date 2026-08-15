using EmployeeTaskManagement.Authorization;
using EmployeeTaskManagement.Dtos.Tasks;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeTaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("by-project/{projectId:int}")]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<IEnumerable<TaskResponse>>> GetByProjectId(int projectId, CancellationToken cancellationToken)
        {
            var tasks = await _taskService.GetByProjectIdAsync(projectId, cancellationToken);
            return Ok(tasks);
        }

        [HttpGet("my-tasks")]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<IEnumerable<TaskResponse>>> GetMyTasks(CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Unable to determine user identity.");
            }

            var tasks = await _taskService.GetAssignedToUserAsync(parsedUserId, cancellationToken);
            return Ok(tasks);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<TaskResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var task = await _taskService.GetByIdAsync(id, cancellationToken);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpPost]
        [Authorize(Policy = Policies.ManagerOrAdmin)]
        public async Task<ActionResult<TaskResponse>> Create([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var createdById))
            {
                return BadRequest("Unable to determine user identity.");
            }

            var created = await _taskService.CreateAsync(request, createdById, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = Policies.ManagerOrAdmin)]
        public async Task<ActionResult<TaskResponse>> Update(int id, [FromBody] UpdateTaskRequest request, CancellationToken cancellationToken)
        {
            var updated = await _taskService.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = Policies.ManagerOrAdmin)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _taskService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
