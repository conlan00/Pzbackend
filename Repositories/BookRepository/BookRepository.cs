using Backend.DTO;
using Backend.Models;
using Clerk.Net.Client.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.BookRepository
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext _libraryContext;
        public BookRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        public async Task<List<BookDto>> GetBooksByUserIdAsync(int userId)
        {
            if (await checkUserIfExist(userId))
            {
                var books = await _libraryContext.Books
         .Join(_libraryContext.Categories,
             book => book.CategoryId,
             category => category.Id,
             (book, category) => new { book, category })
         .Join(_libraryContext.Borrows,
             bc => bc.book.Id,
             borrow => borrow.BookId,
             (bc, borrow) => new { bc.book, bc.category, borrow })
         .Join(_libraryContext.Users,
             bcb => bcb.borrow.UserId,
             user => user.Id,
             (bcb, user) => new
             {
                 bcb.book.Id,
                 bcb.book.Title,
                 bcb.book.Publisher,
                 bcb.book.Author,
                 bcb.book.Description,
                 bcb.book.Cover,
                 bcb.category.CategoryName,
                 bcb.borrow.UserId
             })
         .Where(result => result.UserId == userId)
         .ToListAsync();

                var bookDtos = books.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Publisher = b.Publisher,
                    Author = b.Author,
                    Description = b.Description,
                    Cover = b.Cover,
                    CategoryName = b.CategoryName
                }).ToList();

                return bookDtos;
            }
            return new List<BookDto>();

        }
        public async Task<List<BookDto>> GetFavouriteBooksByUserIdAsync(int userId)
        {
            if (await checkUserIfExist(userId))
            {
                var books = await _libraryContext.Books
                    .Join(_libraryContext.Categories,
                        book => book.CategoryId,
                        category => category.Id,
                        (book, category) => new { book, category })
                    .Join(_libraryContext.LikedBooks,  // Zmiana na LikedBooks
                        bc => bc.book.Id,
                        likedBook => likedBook.BookId,
                        (bc, likedBook) => new { bc.book, bc.category, likedBook })
                    .Join(_libraryContext.Users,
                        bcl => bcl.likedBook.UserId,  // Zmieniamy UserId, bo w LikedBook jest UserId
                        user => user.Id,
                        (bcl, user) => new
                        {
                            bcl.book.Id,
                            bcl.book.Title,
                            bcl.book.Publisher,
                            bcl.book.Author,
                            bcl.book.Description,
                            bcl.book.Cover,
                            bcl.category.CategoryName,
                            bcl.likedBook.UserId
                        })
                    .Where(result => result.UserId == userId)  // Filtrujemy po userId
                    .ToListAsync();

                // Mapowanie wyników na BookDto
                var bookDtos = books.Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Publisher = b.Publisher,
                    Author = b.Author,
                    Description = b.Description,
                    Cover = b.Cover,
                    CategoryName = b.CategoryName
                }).ToList();

                return bookDtos;
            }

            return new List<BookDto>();

        }
        public async Task<Borrow?> returnBorrow(int userId, int bookId)
        {
            var borrow = await _libraryContext.Borrows.FirstOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId);
            return borrow;
        }
        public async Task<bool> setReturnTime(Borrow borrow, DateTime time)
        {

            if (borrow == null)
            {
                return false;
            }

            borrow.ReturnTime = time;

            await _libraryContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> setLoyaltyPoints(int points, int userId)
        {
            var userExist = await _libraryContext.Users.FindAsync(userId);
            if (userExist == null)
            {
                return false;
            }
            else
            {
                userExist.Points += points;
                await _libraryContext.SaveChangesAsync();
                return true;
            }
        }


        private async Task<bool> checkUserIfExist(int userId)
        {
            var userExist = await _libraryContext.Users.FindAsync(userId);
            return userExist != null;
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _libraryContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}