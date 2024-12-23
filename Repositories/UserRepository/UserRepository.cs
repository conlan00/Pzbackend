using Backend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.UserRepository
{
    public class UserRepository: IUserRepository
    {
        private readonly LibraryContext _libraryContext;
        public UserRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public async Task<User?> Login()
        {
           var user= await _libraryContext.Users.FirstOrDefaultAsync();
            return user;
        }
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _libraryContext.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _libraryContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _libraryContext.Users.Update(user);
            return await _libraryContext.SaveChangesAsync() > 0;
        }
    }
}
