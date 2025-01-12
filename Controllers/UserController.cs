using Backend.Models;
using Backend.Repositories.BookRepository;
using Backend.Repositories.UserRepository;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Services.UserService;


namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
 
   

        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;

        public UserController(IUserService userService, IUserRepository userRepository, IBookRepository bookRepository) // Poprawiony konstruktor
        {
            _userService = userService;

            _userRepository = userRepository;
            _bookRepository = bookRepository;
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

        //[Authorize]
        [HttpPost("register")]
        public async Task<ActionResult>Register(User user)
        {
            var registeredUser = await _userRepository.Register(user);
            if (registeredUser!=0)
            {
                return Ok(new
                {id = registeredUser});
            }
            else
            {
                return BadRequest("Uzytkownik istnieje w bazie");
            }
            
        }
        [HttpGet("login")]
        public async Task<ActionResult> Login(string name)
        {
            var idUser = await _userRepository.GetUserIdByNameAsync(name);
            if (idUser!=null)
            {
                return Ok(new
                { id = idUser });
            }
            else
            {
                return BadRequest("Uzytkownik nie istnieje");
            }
        }
        [HttpGet("borrows-history")]
        public async Task<ActionResult> Borrows(int idUser)
        {
            var books = await _bookRepository.GetHistoryBorrowsBooksByUserIdAsync(idUser);
            if (books.Count!=0)
            {       
                return Ok(books);
            }
            else
            {
                return BadRequest("Nie masz zadnych ksiazek w historii wypozyczen");
            }

        }
        [HttpGet("borrows-time")]
        public async Task<ActionResult> BorrowsTime(int idUser)
        {
            var books = await _bookRepository.GetActualBorrowBooksByUserIdAsync(idUser);
            if (books.Count != 0)
            {
                return Ok(books);
            }
            else
            {
                return BadRequest("Nie masz zadnych aktualnie wypozyczonych ksiazek");
            }

        }
        [HttpGet("loyalty-points")]
        public async Task<ActionResult> GetLoyaltyPoints(int idUser)
        {
            var points = await _userRepository.GetLoyaltyPoints(idUser);
           if ( points== -1)
            {
                return BadRequest("Uzytkownik nie istnieje w bazie");
            }
            else
            {
                return Ok(new
                {
                    points = points
                });
            }
        }
        [HttpGet("favourite-books")]
        public async Task<ActionResult> GetFavouriteBooks(int idUser)
        {
            var favouriteBooks = await _bookRepository.GetFavouriteBooksByUserIdAsync(idUser);
            if (favouriteBooks.Count != 0)
            {
                return Ok(favouriteBooks);
            }
            else
            {
                return BadRequest("Nie polubiłeś żadnych książek");
            }
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
