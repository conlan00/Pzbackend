using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Backend.Services.BorrowService;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IBorrowService _borrowService;
        private readonly ILogger<RentalController> _logger;

        public RentalController(IBorrowService borrowService, ILogger<RentalController> logger)
        {
            _borrowService = borrowService;
            _logger = logger;
        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook(int userId, int bookId, int shelterId)
        {
            _logger.LogInformation("Processing borrow request: UserId={UserId}, BookId={BookId}, ShelterId={ShelterId}", userId, bookId, shelterId);

            try
            {
                var borrow = await _borrowService.BorrowBookAsync(userId, bookId, shelterId);

                _logger.LogInformation("Borrow transaction completed successfully for UserId={UserId}, BookId={BookId}, ShelterId={ShelterId}", userId, bookId, shelterId);
                return Ok(new { message = "Book successfully borrowed.", borrow });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the borrowing request for UserId={UserId}, BookId={BookId}, ShelterId={ShelterId}.", userId, bookId, shelterId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
