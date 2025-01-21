using Backend.Models;

namespace Backend.Services.BorrowService
{
    public interface IBorrowService
    {
        Task<Borrow> AddBorrowAsync(Borrow borrow);
        Task<bool> ExtendBorrowAsync(int borrowId, int additionalDays);
        Task<Borrow?> GetBorrowRecordAsync(int userId, int bookId);
        Task<bool> DeleteBorrowRecordAsync(Borrow borrow);
        Task<Borrow> BorrowBookAsync(int userId, int bookId, int shelterId);

    }
}
