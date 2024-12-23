
public interface IUserService
{
    Task<bool> AddPointsToUserAsync(int userId, int pointsToAdd);
    Task<int> ReturnBookAsync(int userId, int bookId);
}