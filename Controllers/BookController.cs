using Backend.Repositories.BookRepository;
using Backend.Repositories.UserRepository;
using Backend.Services.BookService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {

        private readonly IBookRepository _bookRepository;
        private readonly IBookService _bookService;
        public BookController(IBookService bookService, IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
            _bookService = bookService;
        }
        //[ApiVersion("2.0")]
        [HttpPatch("return")]
        public async Task<ActionResult> ReturnBook(int userId, int bookId)
        {
            var borrows = await _bookService.ReturnBook(userId, bookId);  
            return Ok(borrows);
        }

    }
}
