using Backend.Models;

namespace Backend.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<int> Register(User user);
        Task<int> GetLoyaltyPoints(int userId);
        
    }
}
