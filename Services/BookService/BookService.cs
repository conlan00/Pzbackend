using Backend.Models;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        // Pobierz ksi¹¿kê z repozytorium
        var book = await _bookRepository.GetBookByIdAsync(id);

        if (book == null)
        {
            return null; // Brak ksi¹¿ki o podanym ID
        }

        // Mapowanie do DTO
        return new BookDto
        {
            Title = book.Title,
            Author = book.Author,
            Cover = book.Cover
        };
    }
}
