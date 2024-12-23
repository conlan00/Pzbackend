using Backend.Models;
using Microsoft.EntityFrameworkCore;

public class BookRepository : IBookRepository
{
    private readonly LibraryContext _libraryContext;

    public BookRepository(LibraryContext context)
    {
        _libraryContext = context;
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _libraryContext.Books.FirstOrDefaultAsync(b => b.Id == id);
    }
}
