using Backend.Models;

namespace Backend.Services.UserService
{
    public interface IUserService
    {
        Task<bool> AddPointsToUserAsync(int userId, int pointsToAdd);
        Task<int> ReturnBookAsync(int userId, int bookId);
        Task<bool> UserExistsAsync(int userId);

    }
}