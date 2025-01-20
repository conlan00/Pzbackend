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
{
    _logger.LogInformation("Processing borrow request: UserId={UserId}, BookId={BookId}, ShelterId={ShelterId}", userId, bookId, shelterId);

    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        var borrow = await _borrowService.BorrowBookAsync(userId, bookId, shelterId);

        // Zatwierdzenie transakcji
        await transaction.CommitAsync();

        _logger.LogInformation("Borrow transaction completed successfully for UserId={UserId}, BookId={BookId}, ShelterId={ShelterId}", userId, bookId, shelterId);
        return Ok(new { message = "Book successfully borrowed.", borrow });
    }
    catch (KeyNotFoundException ex)
    {
        _logger.LogWarning(ex.Message);
        await transaction.RollbackAsync();
        return NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while processing the borrowing request for UserId={UserId}, BookId={BookId}, ShelterId={ShelterId}.", userId, bookId, shelterId);
        await transaction.RollbackAsync();
        return StatusCode(500, "An error occurred while processing your request.");
    }
}


    }
}
