using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.Models;
using Backend.Services.BorrowService;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IBorrowService _borrowService;
        private readonly ILogger<RentalController> _logger;

        public RentalController(LibraryContext context, IBorrowService borrowService, ILogger<RentalController> logger)
        {
            _context = context;
            _borrowService = borrowService;
            _logger = logger;
        }

        [HttpPost("borrow")]
public async Task<IActionResult> BorrowBook(int userId, int bookId, int shelterId)
{ //!!!!!!!!!!Sprawdzic z triggerem 2
    _logger.LogInformation("Processing borrow request: UserId={UserId}, BookId={BookId}, ShelterId={ShelterId}", userId, bookId, shelterId);

  /*  try
    {*/
        // Sprawdzenie, czy książka istnieje w danym Shelterze
        var bookShelter = await _context.Books
            .FirstOrDefaultAsync(bs => bs.Id == bookId && bs.ShelterId == shelterId);

        if (bookShelter == null)
        {
            _logger.LogWarning("Book with ID {BookId} is not available in Shelter with ID {ShelterId}.", bookId, shelterId);
            return NotFound("The specified book is not available in the given shelter.");
        }

        _logger.LogInformation("Book with ID {BookId} found in Shelter with ID {ShelterId}. Proceeding to remove from Shelter and borrow.", bookId, shelterId);



        // Tworzenie nowego rekordu Borrow
        var borrow = new Borrow
        {
            UserId = userId,
            BookId = bookId,
/*            ShelterId = shelterId,
            ShelterId2 = shelterId,*/
            
            BeginDate = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddDays(14) // Wypożyczenie na 14 dni
        };

        _context.Borrows.Add(borrow);

        // Usunięcie rekordu z tabeli BookShelter
        //_context.BookShelters.Remove(bookShelter);
        //_logger.LogInformation("Book with ID {BookId} removed from Shelter with ID {ShelterId}.", bookId, shelterId);

        // Zapis zmian w bazie
        await _context.SaveChangesAsync();

        return Ok(new { message = "Book successfully borrowed.", borrow });
/*    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while processing the borrowing request for UserId={UserId}, BookId={BookId}, ShelterId={ShelterId}.", userId, bookId, shelterId);
        return StatusCode(500, "An error occurred while processing your request.");
    }*/
}


    }
}
