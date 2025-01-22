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
            // Walidacja wspó³rzêdnych geograficznych
            if (userLat < -90 || userLat > 90)
            {
                return BadRequest("Szerokoœæ geograficzna (userLat) musi byæ w zakresie od -90 do 90 stopni.");
            }

            if (userLong < -180 || userLong > 180)
            {
                return BadRequest("D³ugoœæ geograficzna (userLong) musi byæ w zakresie od -180 do 180 stopni.");
            }

            // Walidacja promienia
            if (radius <= 0)
            {
                return BadRequest("Promieñ (radius) musi byæ wartoœci¹ dodatni¹.");
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
