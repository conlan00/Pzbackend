using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class BookShelterRepository
    {
        private readonly LibraryContext _context;

        public BookShelterRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetBooksInShelterAsync(int shelterId)
        {
            return await _context.BookShelters
                .Where(bs => bs.ShelterId == shelterId)
                .Include(bs => bs.Book)
                .Select(bs => bs.Book)
                .ToListAsync();
        }
    }
}
