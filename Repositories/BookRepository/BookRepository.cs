﻿using Backend.DTO;
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



        public async Task<List<BookDto>> GetHistoryBorrowsBooksByUserIdAsync(int userId)
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
                 bcb.borrow.UserId,
                 bcb.borrow.ReturnTime
             })
         .Where(result => result.UserId == userId && result.ReturnTime != null)
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

        public async Task<List<BookDtoDays>> GetActualBorrowBooksByUserIdAsync(int userId)
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
                     bcb.borrow.UserId,
                     bcb.borrow.BeginDate,
                     bcb.borrow.EndTime,
                     bcb.borrow.ReturnTime
                 })
             .Where(result => result.UserId == userId && result.ReturnTime == null)
             .ToListAsync();
                

                    var bookDtos = books.Select(b => new BookDtoDays
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Publisher = b.Publisher,
                        Author = b.Author,
                        Description = b.Description,
                        Cover = b.Cover,
                        CategoryName = b.CategoryName,
                        Days=(int)(b.EndTime-DateTime.UtcNow).TotalDays
                    }).ToList();

                    return bookDtos;
            }
            return new List<BookDtoDays>();

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
            var borrow = await _libraryContext.Borrows.FirstOrDefaultAsync(x => x.BookId == bookId && x.UserId == userId) ?? null;
            return borrow;
        }
        public async Task<bool> setReturnTime(Borrow borrow, DateTime time, int ShelterId)
        {

            if (borrow == null)
            {
                return false;
            }

            borrow.ReturnTime = time;
            borrow.ReturnShelterId = ShelterId;
            await _libraryContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> addBookArrival(int userId, int bookId, int shelterID)
        {
            var newBookArrival = new BookArrival
            {
                DateTime = DateTime.UtcNow,
                UserId = userId,
                BookId = bookId,
                ShelterId = shelterID
            };
            _libraryContext.Add(newBookArrival);
            return true;
        }
       // public async Task<Book>
/*        public async Task<bool> setShelter(Borrow borrow, int idShelter)
        {

            if (borrow == null)
            {
                return false;
            }

            borrow.ShelterId2 = idShelter;

            await _libraryContext.SaveChangesAsync();
            return true;
        }*/



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

  /*      public async Task<bool> addBookShelter(int bookId, int shelterId)
        {
            var newBookShelter = new BookShelter
            {
                BookId = bookId,
                ShelterId = shelterId
            };
            _libraryContext.Add(newBookShelter);
            await _libraryContext.SaveChangesAsync();
            return true;
        }*/

        private async Task<bool> checkUserIfExist(int userId)
        {
            var userExist = await _libraryContext.Users.FindAsync(userId);
            return userExist != null;
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _libraryContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        }

        //!!!!!!! do sprawdzenia 

        public async Task<IEnumerable<Book>> GetBooksByShelterAndCategoriesAsync(int shelterId, List<int>? categoryIds = null)
        {
            // Pobierz książki w danej budce
            var query = _libraryContext.Books
                .Where(bs => bs.ShelterId == shelterId);

            // Filtruj po kategoriach, jeśli są podane
            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(book => categoryIds.Contains(book.CategoryId));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _libraryContext.Categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                })
                .ToListAsync();
        }
        public async Task<bool> GiveLike(int userId, int bookId)
        {
            var newLikedBook = new LikedBook
            {
                UserId = userId,
                BookId = bookId

            };

            await _libraryContext.LikedBooks.AddAsync(newLikedBook);
            await _libraryContext.SaveChangesAsync();
            return true;
        }

    }
}