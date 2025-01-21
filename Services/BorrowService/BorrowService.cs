using Backend.Models;
using Backend.Repositories.BorrowRepository;

namespace Backend.Services.BorrowService
{
    public class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepository;

        public BorrowService(IBorrowRepository borrowRepository)
        {
            _borrowRepository = borrowRepository;
        }

        public async Task<Borrow?> GetBorrowRecordAsync(int userId, int bookId)
        {
            // Poprawne przekazanie parametrów do metody _borrowRepository.GetBorrowRecordAsync
            return await _borrowRepository.GetBorrowRecordAsync(userId, bookId);
        }

        public async Task<bool> ExtendBorrowAsync(int borrowId, int additionalDays)
        {
            var borrow = await _borrowRepository.GetBorrowRecordAsync(borrowId, 0);
            if (borrow == null)
            {
                throw new Exception("Borrow record not found.");
            }

            return await _borrowRepository.ExtendBorrowAsync(borrow, additionalDays);
        }

        public async Task<Borrow> AddBorrowAsync(Borrow borrow)
        {
            return await _borrowRepository.AddBorrowAsync(borrow);
        }

        public async Task<bool> DeleteBorrowRecordAsync(Borrow borrow)
        {
            return await _borrowRepository.DeleteBorrowRecordAsync(borrow);
        }

        public async Task<Borrow> BorrowBookAsync(int userId, int bookId, int shelterId)
    {
        // Sprawdzenie, czy książka jest dostępna w budce
        var book = await _borrowRepository.GetBookInShelterAsync(bookId, shelterId);

        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {bookId} is not available in Shelter with ID {shelterId}.");
        }

        // Tworzenie rekordu Borrow
        var borrow = new Borrow
        {
            UserId = userId,
            BookId = bookId,
            BeginDate = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddDays(14)
        };

        await _borrowRepository.AddBorrowAsync(borrow);

        // Zapis zmian
        await _borrowRepository.SaveChangesAsync();

        return borrow;
    }
    }
}
