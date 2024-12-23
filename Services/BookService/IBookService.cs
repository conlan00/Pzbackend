using Backend.Models;

namespace Backend.Services.BookService
{
    public interface IBookService
    {
        Task<bool> ReturnBook(int userId, int bookId);

    }
}
