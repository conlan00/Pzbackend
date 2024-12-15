using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        [HttpPost("set")]
        public IActionResult SetLocation()
        {
            return Ok(new { Message = "Location set successfully." });
        }

        [HttpPost("input")]
        public IActionResult InputLocation([FromBody] string address)
        {
            return Ok(new { Address = address, Status = "Location saved." });
        }

        [HttpGet("/settings/location")]
        public IActionResult GoToSettings()
        {
            return Ok(new { Message = "Navigate to GPS settings." });
        }

        [HttpPost("set-default")]
        public IActionResult SetDefaultLocation()
        {
            return Ok(new { Message = "Default location set." });
        }

        [HttpGet("manual-input")]
        public IActionResult ManualInputLocation()
        {
            return Ok(new { Message = "Manual location entry enabled." });
        }
    }
}
