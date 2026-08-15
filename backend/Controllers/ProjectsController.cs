using EmployeeTaskManagement.Authorization;
using EmployeeTaskManagement.Dtos.Projects;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeTaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var projects = await _projectService.GetAllAsync(cancellationToken);
            return Ok(projects);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<ProjectResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var project = await _projectService.GetByIdAsync(id, cancellationToken);
            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        [HttpPost]
        [Authorize(Policy = Policies.ManagerOrAdmin)]
        public async Task<ActionResult<ProjectResponse>> Create([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var createdById))
            {
                return BadRequest("Unable to determine user identity.");
            }

            var created = await _projectService.CreateAsync(request, createdById, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = Policies.ManagerOrAdmin)]
        public async Task<ActionResult<ProjectResponse>> Update(int id, [FromBody] UpdateProjectRequest request, CancellationToken cancellationToken)
        {
            var updated = await _projectService.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = Policies.ManagerOrAdmin)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _projectService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
