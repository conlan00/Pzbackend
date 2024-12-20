using Backend.Models;
using Backend.Repositories.UserRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
      /*  private static readonly string[] RentalSummaries = new[]
   {
        "Book 1", "Book 2", "Book 3", "Book 4", "Book 5"
    };

        private static readonly List<User> Users = new List<User>
    {
        new User { Name = "admin", Token = "password123" }
    };

        [HttpGet("rentals")]
        public IEnumerable<string> GetRentals()
        {
            return RentalSummaries;
        }

        [HttpGet("loyalty-points")]
        public int GetLoyaltyPoints()
        {
            return 150; // Example static points
        }

        [HttpGet("favorite-books")]
        public IEnumerable<string> GetFavoriteBooks()
        {
            return new[] { "Favorite Book 1", "Favorite Book 2" };
        }

        [HttpPost("loyalty-points/add")]
        public IActionResult AddLoyaltyPoints()
        {
            return Ok(new { Message = "80 loyalty points added!" });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (Users.Any(u => u.Name == user.Name))
            {
                return BadRequest(new { Message = "Username already exists." });
            }

            Users.Add(user);
            return Ok(new { Message = "User registered successfully!" });
        }
*/
        [HttpPost("login")]
        public async Task<ActionResult<User>>Login()
        {
            var Data = await _userRepository.Login();
            return Ok(Data);
        }
/*
        private string GenerateToken(string username)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{DateTime.Now}"));
        }*/
    }
}
