using Backend.Models;
using Backend.Repositories.BookRepository;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Backend.Repositories;
using Backend.Repositories.PointsRepository;
using Backend.Backend.Repositories.PointsRepository;

namespace Backend.Services.BookService
{
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
        private readonly LibraryContext _libraryContext;
        private readonly IPointsRepository _pointsRepository;

        public BookService(IBookRepository bookRepository, LibraryContext libraryContext, IPointsRepository pointsRepository)
    {
        _bookRepository = bookRepository;
            _libraryContext = libraryContext;
            _pointsRepository = pointsRepository;
    }

        public async Task<int> ReturnBook(int userId, int bookId, int shelterId)
        {
            // Using transaction to ensure atomicity
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();
            try
            {
                var borrow = await _bookRepository.returnBorrow(userId, bookId);
                if (borrow != null)
                {
                    if (borrow.ReturnTime == null)
                    {
                        // Set the return time and shelter
                        await _bookRepository.setReturnTime(borrow, DateTime.UtcNow, shelterId);

                        // Calculate return details
                        int returnDays = (int)(DateTime.UtcNow - borrow.BeginDate).TotalDays;
                        int returnDeadlineInDays = (int)(borrow.EndTime - borrow.BeginDate).TotalDays;
                        int points = 0;

                        if (returnDays <= 7)
                        {
                            points = 30;
                            await _bookRepository.setLoyaltyPoints(points, userId);
                            return points;
                        }
                        else if (returnDays > returnDeadlineInDays)
                        {
                            var delayDays = returnDays - returnDeadlineInDays;
                            int firstWeekPenalty = Math.Min(delayDays, 7) * -5;
                            int additionalDaysPenalty = Math.Max(delayDays - 7, 0) * -15;
                            points = firstWeekPenalty + additionalDaysPenalty;
                            await _bookRepository.setLoyaltyPoints(points, userId);
                            return points;
                        }

                        // Add a record to OperationHistory
                        if (points != 0)
                        {
                            await _pointsRepository.addOperationHistory(new OperationHistory
                            {
                                UserId = userId,
                                OperationDescription = points.ToString(), // Zapisz tylko liczbę punktów
                                DateTime = DateTime.UtcNow
                            });
                        }

                    }

                    await transaction.CommitAsync();
                    //return 1234567890;
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return 123456789;
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
