using Backend.DTO;
using Backend.Models;
using System.Data;

namespace Backend.Repositories.BookRepository
{
public interface IBookRepository
{
        Task<List<BookDto>> GetBooksByUserIdAsync(int userId);
        Task<List<BookDto>> GetFavouriteBooksByUserIdAsync(int userId);
        Task<Borrow> returnBorrow(int userId, int bookId);
        Task<bool> setReturnTime(Borrow borrow,DateTime time);
        Task<bool> setLoyaltyPoints(int points,int userId);
        Task<Book?> GetBookByIdAsync(int id);
    }
}
