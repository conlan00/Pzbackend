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
                // Obs�uga b��d�w, np. logowanie
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
                // Zwraca b��d 400, je�li walidacja nie przesz�a
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Obs�uga innych b��d�w
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
