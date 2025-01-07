public class BorrowService : IBorrowService
{
    private readonly IBorrowRepository _borrowRepository;

    public BorrowService(IBorrowRepository borrowRepository)
    {
        _borrowRepository = borrowRepository;
    }

    public async Task<bool> ExtendBorrowAsync(int userId, int bookId, int additionalDays)
    {
        // Pobierz rekord Borrow
        var borrow = await _borrowRepository.GetBorrowRecordAsync(userId, bookId);

        if (borrow == null)
        {
            throw new Exception("Borrow record not found.");
        }

        // Przedłuż wypożyczenie
        return await _borrowRepository.ExtendBorrowAsync(borrow, additionalDays);
    }
}