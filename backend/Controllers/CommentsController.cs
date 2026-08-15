using EmployeeTaskManagement.Authorization;
using EmployeeTaskManagement.Dtos.Comments;
using EmployeeTaskManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeTaskManagement.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId:int}/comments")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<IEnumerable<CommentResponse>>> GetByTaskId(int taskId, CancellationToken cancellationToken)
        {
            var comments = await _commentService.GetByTaskIdAsync(taskId, cancellationToken);
            return Ok(comments);
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<CommentResponse>> GetById(int id, CancellationToken cancellationToken)
        {
            var comment = await _commentService.GetByIdAsync(id, cancellationToken);
            if (comment == null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpPost]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<CommentResponse>> Create(int taskId, [FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Unable to determine user identity.");
            }

            var created = await _commentService.CreateAsync(request, parsedUserId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { taskId = taskId, id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<ActionResult<CommentResponse>> Update(int id, [FromBody] UpdateCommentRequest request, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Unable to determine user identity.");
            }

            var updated = await _commentService.UpdateAsync(id, request, parsedUserId, cancellationToken);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = Policies.EmployeeOrManagerOrAdmin)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Unable to determine user identity.");
            }

            await _commentService.DeleteAsync(id, parsedUserId, cancellationToken);
            return NoContent();
        }
    }
}
