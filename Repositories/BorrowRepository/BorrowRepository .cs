using Backend.Models;
using Microsoft.EntityFrameworkCore;

public class BorrowRepository : IBorrowRepository
{
    private readonly LibraryContext _context;

    public BorrowRepository(LibraryContext context)
    {
        _context = context;
    }

    public async Task<Borrow?> GetBorrowRecordAsync(int userId, int bookId)
    {
        return await _context.Borrows
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BookId == bookId);
    }

    public async Task DeleteBorrowRecordAsync(Borrow borrow)
    {
        _context.Borrows.Remove(borrow);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExtendBorrowAsync(Borrow borrow, int additionalDays)
    {
        borrow.EndTime = borrow.EndTime.AddDays(additionalDays);
        return await _context.SaveChangesAsync() > 0;
    }
}
