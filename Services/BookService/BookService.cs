using Backend.Models;
using Backend.Repositories.BookRepository;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.BookService
{
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
        private readonly LibraryContext _libraryContext;

        public BookService(IBookRepository bookRepository, LibraryContext libraryContext)
    {
        _bookRepository = bookRepository;
            _libraryContext = libraryContext;
    }

        public async Task<bool> ReturnBook(int userId, int bookId, int ShelterId)
        {
            // using trigger -->T_AfterUpdate_Borrow
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();
            try
            {
                var borrow = await _bookRepository.returnBorrow(userId, bookId);
                if (borrow != null)
                {
                    if (borrow.ReturnTime == null)
                    {

                        await _bookRepository.setReturnTime(borrow, DateTime.UtcNow,ShelterId);
                        //await _bookRepository.setShelter(borrow, ShelterId);
                        // dodac wiersz do book shelter
                        //await _bookRepository.addBookShelter(bookId, ShelterId);

                        int returnDays = (int)(DateTime.UtcNow - borrow.BeginDate).TotalDays;
                        int returnDeadlineInDays = (int)(borrow.EndTime - borrow.BeginDate).TotalDays;

                        if (returnDays <= 7)
                        {
                            await _bookRepository.setLoyaltyPoints(30, userId);
                        }
                        else if (returnDays > returnDeadlineInDays)
                        {
                            var delayDays = returnDays - returnDeadlineInDays;
                            int firstWeekPenalty = Math.Min(delayDays, 7) * -5;
                            int additionalDaysPenalty = Math.Max(delayDays - 7, 0) * -15;
                            await _bookRepository.setLoyaltyPoints((firstWeekPenalty + additionalDaysPenalty), userId);
                        }

                    }
                    await transaction.CommitAsync();

                    return true;
                }
            } catch (Exception ex)
        {
                await transaction.RollbackAsync();
                throw;
            }
            return false;

        }
        public async Task<BookDto2?> GetBookByIdAsync(int id)
        {
            // Pobierz książkę z repozytorium
            var book = await _bookRepository.GetBookByIdAsync(id);

            if (book == null)
            {
                return null; // Brak książki o podanym ID
            }

            // Mapowanie do DTO
            return new BookDto2
            {
                Title = book.Title,
                Author = book.Author,
                Cover = book.Cover
            };
        }


        public async Task<IEnumerable<Book>> GetFilteredBooksAsync(int shelterId, List<int>? categoryIds = null)
        {
            return await _bookRepository.GetBooksByShelterAndCategoriesAsync(shelterId, categoryIds);
        }

public async Task<object> AddBookWithGoogleApiAsync(AddBookRequest request)
{
    // Rozpoczęcie transakcji
    using var transaction = await _libraryContext.Database.BeginTransactionAsync();

    try
    {
        // Pobieranie danych z Google Books API
        string googleBooksApiUrl = $"https://www.googleapis.com/books/v1/volumes?q=intitle:{request.Title}&inauthor:{request.Author}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(googleBooksApiUrl);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to fetch data from Google Books API.");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var googleBooksData = JsonDocument.Parse(jsonResponse);

        var volumeInfo = googleBooksData.RootElement.GetProperty("items")[0].GetProperty("volumeInfo");

        // Pobranie danych z API
        string description = volumeInfo.TryGetProperty("description", out var desc) ? desc.GetString() : "No description provided";
        string? coverUrl = volumeInfo.TryGetProperty("imageLinks", out var imageLinks) && imageLinks.TryGetProperty("thumbnail", out var thumbnail)
            ? thumbnail.GetString()
            : null;
        string categoryName = volumeInfo.TryGetProperty("categories", out var categoriesJson) && categoriesJson.GetArrayLength() > 0
            ? categoriesJson[0].GetString() ?? "Uncategorized"
            : "Uncategorized";

        // Sprawdzenie lub dodanie kategorii
        var category = await _libraryContext.Categories
            .FirstOrDefaultAsync(c => c.CategoryName == categoryName);
        if (category == null)
        {
            category = new Category { CategoryName = categoryName };
            _libraryContext.Categories.Add(category);
            await _libraryContext.SaveChangesAsync();
        }

        // Dodanie książki do bazy danych
        var newBook = new Book
        {
            Title = request.Title,
            Author = request.Author,
            Publisher = request.Publisher,
            Description = description,
            Cover = coverUrl,
            CategoryId = category.Id
        };

        _libraryContext.Books.Add(newBook);
        await _libraryContext.SaveChangesAsync();

        // Dodanie rekordu w BookArrival
        var bookArrival = new BookArrival
        {
            UserId = request.UserId,
            BookId = newBook.Id,
            ShelterId = request.ShelterId,
            DateTime = DateTime.UtcNow
        };

        _libraryContext.BookArrivals.Add(bookArrival);
        await _libraryContext.SaveChangesAsync();

        // Zatwierdzenie transakcji
        await transaction.CommitAsync();

        return new
        {
            Message = "Book and category added successfully.",
            BookId = newBook.Id,
            Category = category.CategoryName,
            CoverUrl = coverUrl,
            Description = description
        };
    }
    catch (Exception ex)
    {
        // Wycofanie transakcji w razie błędu
        await transaction.RollbackAsync();
        throw;
    }
}








    }
}
