using Backend.Models;
using Backend.Repositories.BorrowRepository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services.BorrowService
{
    public class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepository;
        private readonly LibraryContext _libraryContext;

        public BorrowService(IBorrowRepository borrowRepository, LibraryContext libraryContext)
        {
            _borrowRepository = borrowRepository;
            _libraryContext = libraryContext;
        }

        public async Task<Borrow?> GetBorrowRecordAsync(int userId, int bookId)
        {
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();

            try
            {
                // Pobranie rekordu wypożyczenia
                var borrow = await _borrowRepository.GetBorrowRecordAsync(userId, bookId);

                if (borrow == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                await transaction.CommitAsync();
                return borrow;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ExtendBorrowAsync(int borrowId, int additionalDays)
        {
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();

            try
            {
                var borrow = await _borrowRepository.GetBorrowRecordAsync(borrowId, 0);
                if (borrow == null)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Borrow record not found.");
                }

                var result = await _borrowRepository.ExtendBorrowAsync(borrow, additionalDays);

                await transaction.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Borrow> AddBorrowAsync(Borrow borrow)
        {
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();

            try
            {
                var addedBorrow = await _borrowRepository.AddBorrowAsync(borrow);
                await _libraryContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return addedBorrow;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteBorrowRecordAsync(Borrow borrow)
        {
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();

            try
            {
                var result = await _borrowRepository.DeleteBorrowRecordAsync(borrow);
                await _libraryContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Borrow> BorrowBookAsync(int userId, int bookId, int shelterId)
        {
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();

            try
            {
                // Sprawdzenie, czy książka jest dostępna w budce
                var book = await _borrowRepository.GetBookInShelterAsync(bookId, shelterId);

                if (book == null)
                {
                    await transaction.RollbackAsync();
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
                await _libraryContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return borrow;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
