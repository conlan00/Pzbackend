using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private static readonly string[] RentalStatuses = new[]
        {
        "Active", "Extended", "Completed"
        };

        [HttpGet("extend")]
        public IEnumerable<string> GetExtend()
        {
            return RentalStatuses;
        }

        [HttpPost("update-duration")]
        public IActionResult UpdateRentalDuration()
        {
            return Ok(new { Message = "Rental duration updated." });
        }

        [HttpPost("confirm-extension")]
        public IActionResult ConfirmExtension()
        {
            return Ok(new { Message = "Rental successfully extended!" });
        }

        [HttpGet("confirmation")]
        public IActionResult GetRentalConfirmation()
        {
            return Ok(new { Book = "Example Book", ReturnDate = DateTime.Now.AddDays(7).ToShortDateString() });
        }
    }
}
