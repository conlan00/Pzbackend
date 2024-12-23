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

    private readonly LibraryContext _context;

    public BookController(LibraryContext context)
    {
        _context = context;
    }

    [HttpGet("user-books/{userId}")]
    public async Task<IActionResult> GetBooksByUser(int userId)
    {
        // Pobierz książki wypożyczone przez użytkownika
        var books = await _context.Borrows
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




        /*private static readonly string[] BookTitles = new[]
        {
        "The Hobbit", "1984", "Moby Dick", "War and Peace"
        };

        [HttpPost("return")]
        public IActionResult ReturnBook()
        {
            return Ok(new { Message = "Book returned successfully!" });
        }

        [HttpPost("return/confirm")]
        public IActionResult ConfirmReturn()
        {
            return Ok(new { PointsEarned = 50 });
        }

        [HttpGet("{id}")]
        public IActionResult GetBookDetails(int id)
        {
            return Ok(new { Id = id, Title = BookTitles[id % BookTitles.Length], Author = "Unknown" });
        }

        [HttpPost("verify")]
        public IActionResult VerifyBook()
        {
            return Ok(new { Message = "Book verified successfully." });
        }

        [HttpPost("ocr")]
        public IActionResult OCRVerification()
        {
            return Ok(new { Status = "OCR Successful", Matched = true });
        }

        [HttpPost("rent/verify")]
        public IActionResult RentVerify()
        {
            return Ok(new { Message = "Book ready for rent verification." });
        }

        [HttpGet("{id}/details")]
        public IActionResult GetBookFullDetails(int id)
        {
            return Ok(new { Title = BookTitles[id % BookTitles.Length], Description = "An amazing book description!" });
        }

        [HttpGet("return/retry")]
        public IActionResult RetryReturn()
        {
            return Ok(new { Message = "Please retry the return process." });
        }
        [HttpGet("filter")]
        public IActionResult FilterBooks([FromQuery] string category, [FromQuery] string language, [FromQuery] string condition)
        {
            return Ok(new
            {
                Category = category ?? "All Categories",
                Language = language ?? "All Languages",
                Condition = condition ?? "Any Condition",
                Books = new[]
                {
                new { Title = "Sample Book 1", Category = category, Language = language, Condition = condition },
                new { Title = "Sample Book 2", Category = category, Language = language, Condition = condition }
            }
            });
        }*/
    }
}
