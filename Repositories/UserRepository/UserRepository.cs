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
    }
}
