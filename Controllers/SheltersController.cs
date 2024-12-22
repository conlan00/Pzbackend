using Backend.Services.ShelterService;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SheltersController : ControllerBase
    {
        private readonly IShelterService _shelterService;

        public SheltersController(IShelterService shelterService)
        {
            _shelterService = shelterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShelters()
        {
            try
            {
                var shelters = await _shelterService.GetAllSheltersAsync();
                return Ok(shelters);
            }
            catch (Exception ex)
            {
                // Obs³uga b³êdów, np. logowanie
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyShelters([FromQuery] double userLat, [FromQuery] double userLong, [FromQuery] double radius = 15)
        {
            if (userLat == 0 || userLong == 0)
            {
                return BadRequest("userLat and userLong are required.");
            }

            try
            {
                var shelters = await _shelterService.GetNearbySheltersAsync(userLat, userLong, radius);
                return Ok(shelters);
            }
            catch (ArgumentException ex)
            {
                // Zwraca b³¹d 400, jeœli walidacja nie przesz³a
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Obs³uga innych b³êdów
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
