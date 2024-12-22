using Backend.Models;

namespace Backend.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> Login();
        Task<IEnumerable<User>> GetAllUsers();
    }
}
