public interface IBorrowService
{
    Task<bool> ExtendBorrowAsync(int userId, int bookId, int additionalDays);
}