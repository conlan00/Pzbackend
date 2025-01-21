using Backend.Repositories.BookRepository;
using Backend.Repositories.UserRepository;
using Backend.Services.BookService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Backend.DTO;
using System.Text.Json;
using Backend.Services.UserService;


namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookService _bookService;
        private readonly LibraryContext _libraryContext;

        public BookController(IBookService bookService, IBookRepository bookRepository, LibraryContext libraryContext)
        {
            _bookRepository = bookRepository;
            _bookService = bookService;
            _libraryContext = libraryContext;
        }

        [HttpPatch("return")]
        public async Task<ActionResult> ReturnBook(int userId, int bookId, int ShelterId)
        {
            var points = await _bookService.ReturnBook(userId, bookId, ShelterId);
            return Ok(new
            {
                points= points,

            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            return Ok(book);
        }
        [HttpPost("give-like")]
        public async Task<IActionResult> GiveLike(int userId, int bookId)
        {
            await _bookRepository.GiveLike(userId, bookId);
            return Ok("Dodano polubienie");
        }




        [HttpGet("user-books/{userId}")]
        public async Task<IActionResult> GetBooksByUser(int userId)
        {
            // Pobierz książki wypożyczone przez użytkownika
            var books = await _libraryContext.Borrows
                .Where(b => b.UserId == userId)
                .Include(b => b.Book)
                .Select(b => new
                {
                    Title = b.Book.Title,
                    Author = b.Book.Author,
                    Cover = b.Book.Cover,
                    ReturnTime = b.ReturnTime
                })
                .ToListAsync();

            if (!books.Any())
            {
                return NotFound(new { Message = $"No books found for user with ID {userId}" });
            }

            return Ok(new
            {
                UserId = userId,
                Books = books
            });
        }

        [HttpPatch("extend")]
        public async Task<IActionResult> ExtendBorrow(int userId, int bookId, [FromQuery] int additionalDays)
        {
            // Pobierz rekord wypożyczenia
            var borrow = await _libraryContext.Borrows
                .FirstOrDefaultAsync(b => b.UserId == userId && b.BookId == bookId);

            if (borrow == null)
            {
                return NotFound(new { Message = $"Borrow record not found for user ID {userId} and book ID {bookId}" });
            }

            // Przedłuż datę zwrotu
            borrow.EndTime = borrow.EndTime.AddDays(additionalDays);

            // Zapisz zmiany
            await _libraryContext.SaveChangesAsync();

            return Ok(new
            {
               
                NewReturnTime = borrow.EndTime
            });
        }


        [HttpGet("filter")]
        public async Task<IActionResult> FilterBooks([FromQuery] int shelterId, [FromQuery] List<int>? categoryIds = null)
        {
            var books = await _bookService.GetFilteredBooksAsync(shelterId, categoryIds);

            if (!books.Any())
            {
                return NotFound(new { Message = "No books found for the given criteria." });
            }

            return Ok(books.Select(book => new
            {
                book.Id,
                book.Title,
                book.Author,
                book.Cover,
                CategoryId = book.CategoryId
            }));
        }

        [HttpGet("all-categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _bookRepository.GetAllCategoriesAsync();

            if (categories == null || !categories.Any())
            {
                return NotFound(new { Message = "Nie znaleziono żadnych kategorii." });
            }

            return Ok(categories.Select(c => new
            {
                c.Id,
                c.CategoryName
            }));
        }

       [HttpPost("add-book")]
        public async Task<IActionResult> AddBook([FromBody] AddBookRequest request)
        {
            try
            {
                var result = await _bookService.AddBookWithGoogleApiAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
