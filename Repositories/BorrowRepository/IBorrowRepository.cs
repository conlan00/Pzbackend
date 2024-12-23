using Backend.Models;

public interface IBorrowRepository
{
    Task<Borrow?> GetBorrowRecordAsync(int userId, int bookId);
    Task DeleteBorrowRecordAsync(Borrow borrow);
}