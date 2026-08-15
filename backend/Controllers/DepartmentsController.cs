using EmployeeTaskManagement.Authorization;
using EmployeeTaskManagement.Dtos.Departments;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetAll(CancellationToken cancellationToken)
        {
            var departments = await _departmentService.GetAllAsync(cancellationToken);
            return Ok(departments);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<DepartmentResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var department = await _departmentService.GetByIdAsync(id, cancellationToken);
            if (department == null)
            {
                return NotFound();
            }

            return Ok(department);
        }

        [HttpPost]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<DepartmentResponse>> Create([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken)
        {
            var created = await _departmentService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<ActionResult<DepartmentResponse>> Update(int id, [FromBody] UpdateDepartmentRequest request, CancellationToken cancellationToken)
        {
            var updated = await _departmentService.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = Policies.AdminOnly)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _departmentService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
