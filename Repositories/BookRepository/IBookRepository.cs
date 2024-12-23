using Backend.Models;

public interface IBookRepository
{
    Task<Book?> GetBookByIdAsync(int id);
}
