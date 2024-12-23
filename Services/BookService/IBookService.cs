using Backend.Models;

namespace Backend.Services.BookService
{
public interface IBookService
{
    Task<BookDto2?> GetBookByIdAsync(int id);
        Task<bool> ReturnBook(int userId, int bookId);

    }
}