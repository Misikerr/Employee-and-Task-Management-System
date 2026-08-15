using Microsoft.AspNetCore.Mvc;

namespace EmployeeTaskManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new { status = "Healthy", timestamp = System.DateTime.UtcNow });
    }
}
