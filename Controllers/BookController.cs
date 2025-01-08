using Backend.Repositories.BookRepository;
using Backend.Repositories.UserRepository;
using Backend.Services.BookService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Backend.DTO;
using System.Text.Json;


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
/*
        [HttpPatch("return")]
        public async Task<ActionResult> ReturnBook(int userId, int bookId, int ShelterId)
        {
            var borrows = await _bookService.ReturnBook(userId, bookId, ShelterId);
            return Ok(borrows);
        }*/

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
/*
    [HttpPost("add-book")]
    public async Task<IActionResult> AddBook([FromBody] AddBookRequest request)
    {
        if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Author))
        {
            return BadRequest(new { Message = "Title and Author are required." });
        }
    
        // Pobierz dane z Google Books API
        string googleBooksApiUrl = $"https://www.googleapis.com/books/v1/volumes?q=intitle:{request.Title}&inauthor:{request.Author}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(googleBooksApiUrl);
        
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode(500, new { Message = "Failed to fetch data from Google Books API." });
        }
        
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var googleBooksData = JsonDocument.Parse(jsonResponse);

        var volumeInfo = googleBooksData.RootElement.GetProperty("items")[0].GetProperty("volumeInfo");
    
        Console.WriteLine($"Google Books API Response: {jsonResponse}");


        // Pobierz opis książki
        string description = volumeInfo.TryGetProperty("description", out var desc) ? desc.GetString() : "Brak opisu";
    
        // Ustaw maksymalną długość opisu
        int maxDescriptionLength = 255; // Wartość znana na podstawie schematu bazy danych
    
        // Przytnij opis, jeśli przekracza maksymalną długość
        if (description.Length > maxDescriptionLength)
        {
            description = description.Substring(0, maxDescriptionLength);
        }
    
        // Pobierz okładkę książki
        string? coverUrl = volumeInfo.TryGetProperty("imageLinks", out var imageLinks) && imageLinks.TryGetProperty("thumbnail", out var thumbnail)
            ? thumbnail.GetString()
            : null;
    
        // Pobierz kategorię książki
        string categoryName = volumeInfo.TryGetProperty("categories", out var categoriesJson) && categoriesJson.GetArrayLength() > 0
            ? categoriesJson[0].GetString() ?? "No Category"
            : "No Category";
    
        // Sprawdź, czy kategoria istnieje w bazie danych
        var category = await _libraryContext.Categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName);
        if (category == null)
        {
            // Jeśli nie ma kategorii, dodaj ją do bazy
            category = new Category { CategoryName = categoryName };
            _libraryContext.Categories.Add(category);
            await _libraryContext.SaveChangesAsync(); // Zapisz nową kategorię
        }
    
        // Stwórz obiekt książki i przypisz kategorię
        var newBook = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Publisher = request.Publisher,
            Description = description,
            Cover = coverUrl,
            CategoryId = category.Id // Przypisz ID kategorii
        };
    
        _libraryContext.Books.Add(newBook);
    
        // Powiąż książkę z budką
        var bookShelter = new BookShelter
        {
            Book = newBook,
            ShelterId = request.ShelterId
        };
        _libraryContext.BookShelters.Add(bookShelter);
    
        await _libraryContext.SaveChangesAsync();
    
        return Ok(new
        {
            Message = "Book and category added successfully.",
            BookId = newBook.Id,
            Category = categoryName
        });
    }*/
    }
}
