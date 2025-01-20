using Backend.Models;

namespace Backend.Repositories.BorrowRepository
{
    public interface IBorrowRepository
    {
        Task<Borrow> AddBorrowAsync(Borrow borrow); // Dodaj tę metodę
        Task<Borrow?> GetBorrowRecordAsync(int userId, int bookId);
        Task<bool> DeleteBorrowRecordAsync(Borrow borrow);
        Task<bool> ExtendBorrowAsync(Borrow borrow, int additionalDays);
        Task<Book?> GetBookInShelterAsync(int bookId, int shelterId);
        Task SaveChangesAsync();

    }
}
