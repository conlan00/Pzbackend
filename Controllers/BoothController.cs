using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Repositories;
using Backend.Repositories.ShelterRepository;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoothController : ControllerBase
    {
        private readonly IShelterRepository _shelterRepository;

        public BoothController(IShelterRepository shelterRepositorsy)
        {
            _shelterRepository = shelterRepositorsy;
        }

        [HttpGet("books-booth/{id}")]
        public async Task<IActionResult> GetBooksInShelter(int id)
        {
            var books = await _shelterRepository.GetBooksInShelterAsync(id);

            if (!books.Any())
            {
                return NotFound(new { Message = $"Nie znaleziono żadnej książki o ID {id}" });
            }

            return Ok(new
            {
                ShelterId = id,
                Books = books.Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Author,
                    b.Description
                })
            });
        }

    }
}
