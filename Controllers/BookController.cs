using Backend.Repositories.BookRepository;
using Backend.Repositories.UserRepository;
using Backend.Services.BookService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult> ReturnBook(int userId, int bookId)
        {
            var borrows = await _bookService.ReturnBook(userId, bookId);
            return Ok(borrows);
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
            borrow.ReturnTime = borrow.ReturnTime?.AddDays(additionalDays);

            // Zapisz zmiany
            await _libraryContext.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Borrow period extended by {additionalDays} days.",
                NewReturnTime = borrow.ReturnTime
            });
        }
    }
}
