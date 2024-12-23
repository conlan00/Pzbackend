using Backend.Models;
using Backend.Repositories.BookRepository;
using Backend.Repositories.UserRepository;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;
        public UserController(IUserRepository userRepository, IBookRepository bookRepository)
        {
            _userRepository = userRepository;
            _bookRepository = bookRepository;
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
        [HttpGet("borrows")]
        public async Task<ActionResult> Borrows(int idUser)
        {
            var books = await _bookRepository.GetBooksByUserIdAsync(idUser);
            if (books.Count!=0)
            {       
                return Ok(books);
            }
            else
            {
                return BadRequest("Nie wypożyczyłeś żadnych książek");
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

    }
}
