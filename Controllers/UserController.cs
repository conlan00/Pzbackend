using Backend.Models;
using Backend.Repositories.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        //private readonly IUserRepository _userRepository;
        //public UserController(IUserRepository userRepository)
        //{
        //    _userRepository = userRepository;
        //}

        //[HttpPost("login")]
        //public async Task<ActionResult<User>>Login()
        //{
        //    var Data = await _userRepository.Login();
        //    return Ok(Data);
        //}

        private readonly IUserService _userService;

        public UserController(IUserService userService) // Poprawiony konstruktor
        {
            _userService = userService;
        }

        [HttpPost("{userId}/add-points")]
        public async Task<IActionResult> AddPointsToUser(int userId, [FromQuery] int pointsToAdd = 80)
        {
            var result = await _userService.AddPointsToUserAsync(userId, pointsToAdd);

            if (!result)
            {
                return Ok(false); // Zwracamy false, gdy użytkownik nie istnieje
            }

            return Ok(true); // Zwracamy true, gdy operacja się powiodła
        }

        [HttpPost("{userId}/return-book/{bookId}")]
        public async Task<IActionResult> ReturnBook(int userId, int bookId)
        {
            try
            {
                var points = await _userService.ReturnBookAsync(userId, bookId);
                return Ok(new { Points = points }); // Zwracamy JSON z liczbą punktów
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}
