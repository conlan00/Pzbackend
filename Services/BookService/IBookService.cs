using Backend.Models;

public interface IBookService
{
    Task<BookDto?> GetBookByIdAsync(int id);
}