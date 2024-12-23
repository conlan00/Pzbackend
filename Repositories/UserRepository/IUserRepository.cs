using Backend.Models;

namespace Backend.Repositories.UserRepository
{
    public interface IUserRepository
    {
       // Task<User?> Login();
        Task<IEnumerable<User>> GetAllUsers();
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserAsync(User user);
        Task<int> Register(User user);
        Task<int> GetLoyaltyPoints(int userId);
        
    }
}
