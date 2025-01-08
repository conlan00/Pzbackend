using Backend.DTO;
using Backend.Models;
using System.Data;

namespace Backend.Repositories.BookRepository
{
public interface IBookRepository
{
        Task<List<BookDto>> GetHistoryBorrowsBooksByUserIdAsync(int userId);
        Task<List<BookDto>> GetFavouriteBooksByUserIdAsync(int userId);
        Task<Borrow> returnBorrow(int userId, int bookId);
        Task<bool> setReturnTime(Borrow borrow,DateTime time);
        Task<bool> setLoyaltyPoints(int points,int userId);
        Task<Book?> GetBookByIdAsync(int id);
        Task<IEnumerable<Book>> GetBooksByShelterAndCategoriesAsync(int shelterId, List<int>? categoryIds = null);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<List<BookDtoDays>> GetActualBorrowBooksByUserIdAsync(int userId);
        Task<bool> setShelter(Borrow borrow, int idShelter);
        Task<bool> addBookShelter(int bookId, int shelterId);
    }
}
