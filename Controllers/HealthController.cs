using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Whispr.Controllers
{
    [ApiController]
    [Route("v1/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet("ping")]
        [EnableRateLimiting("health")]
        public IActionResult Get()
        {
            return Ok(new { status = "ok" });
        }
    }
}
