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
            // Walidacja wsp�rz�dnych geograficznych
            if (userLat < -90 || userLat > 90)
            {
                return BadRequest("Szeroko�� geograficzna (userLat) musi by� w zakresie od -90 do 90 stopni.");
            }

            if (userLong < -180 || userLong > 180)
            {
                return BadRequest("D�ugo�� geograficzna (userLong) musi by� w zakresie od -180 do 180 stopni.");
            }

            // Walidacja promienia
            if (radius <= 0)
            {
                return BadRequest("Promie� (radius) musi by� warto�ci� dodatni�.");
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
