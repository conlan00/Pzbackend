using Backend.Models;
using Backend.Repositories.BookRepository;

namespace Backend.Services.BookService
{
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
        private readonly LibraryContext _libraryContext;

        public BookService(IBookRepository bookRepository, LibraryContext libraryContext)
    {
        _bookRepository = bookRepository;
            _libraryContext = libraryContext;
    }

        public async Task<bool> ReturnBook(int userId, int bookId, int ShelterId)
    {
            using var transaction = await _libraryContext.Database.BeginTransactionAsync();
            try
            {
                var borrow = await _bookRepository.returnBorrow(userId, bookId);
                if (borrow != null)
                {
                    if (borrow.ReturnTime == null)
                    {

                        await _bookRepository.setReturnTime(borrow, DateTime.UtcNow);
                        await _bookRepository.setShelter(borrow, ShelterId);
                        // dodac wiersz do book shelter
                        await _bookRepository.addBookShelter(bookId, ShelterId);

                        int returnDays = (int)(DateTime.UtcNow - borrow.BeginDate).TotalDays;
                        int returnDeadlineInDays = (int)(borrow.EndTime - borrow.BeginDate).TotalDays;

                        if (returnDays <= 7)
                        {
                            await _bookRepository.setLoyaltyPoints(30, userId);
                        }
                        else if (returnDays > returnDeadlineInDays)
        {
                            var delayDays = returnDays - returnDeadlineInDays;
                            int firstWeekPenalty = Math.Min(delayDays, 7) * -5;
                            int additionalDaysPenalty = Math.Max(delayDays - 7, 0) * -15;
                            await _bookRepository.setLoyaltyPoints((firstWeekPenalty + additionalDaysPenalty), userId);
                        }

        }
                    await transaction.CommitAsync();

                    return true;
                }
            } catch (Exception ex)
        {
                await transaction.RollbackAsync();
                throw;
            }
            return false;

        }
        public async Task<BookDto2?> GetBookByIdAsync(int id)
        {
            // Pobierz książkę z repozytorium
            var book = await _bookRepository.GetBookByIdAsync(id);

            if (book == null)
            {
                return null; // Brak książki o podanym ID
            }

            // Mapowanie do DTO
            return new BookDto2
            {
                Title = book.Title,
                Author = book.Author,
                Cover = book.Cover
            };
        }


        public async Task<IEnumerable<Book>> GetFilteredBooksAsync(int shelterId, List<int>? categoryIds = null)
        {
            return await _bookRepository.GetBooksByShelterAndCategoriesAsync(shelterId, categoryIds);
        }
    }
    
}
