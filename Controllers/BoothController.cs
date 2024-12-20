using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoothController : ControllerBase
    {
        /*[HttpGet]
        public IActionResult GetNearbyBooths([FromQuery] int radius)
        {
            return Ok(new[] { "Booth 1 within " + radius + "km", "Booth 2 within " + radius + "km" });
        }

        [HttpGet("{id}/add-book")]
        public IActionResult AddBookToBooth(int id)
        {
            return Ok(new { BoothId = id, Message = "Navigate to add a new book." });
        }

        [HttpGet("{id}")]
        public IActionResult GetBoothDetails(int id)
        {
            return Ok(new { BoothId = id, Name = "Booth " + id, BooksAvailable = 20 });
        }

        [HttpGet("summary")]
        public IActionResult GetBoothSummary()
        {
            return Ok(new[] { "Booth 1", "Booth 2", "Booth 3" });
        }

        [HttpPost("filters")]
        public IActionResult ApplyFilters()
        {
            return Ok(new { Message = "Filters applied successfully." });
        }*/
    }
}
