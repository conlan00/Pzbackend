using Backend.Dto;
using Backend.Models;
//using Clerk.Net.Client.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly LibraryContext _libraryContext;
        public UserRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public async Task<int> Register(User user)
        {
            // var user= await _libraryContext.Users.FirstOrDefaultAsync();
            if (!await checkUserIfExist(user.Id)) {
                var userToAdd = new User
                {
                    Name = user.Name.ToLower(),
                    Points = 0,
                    Token = user.Token.ToLower()
                };
                await _libraryContext.Users.AddAsync(userToAdd);
                await _libraryContext.SaveChangesAsync();

                return userToAdd.Id;

            }
            return 0;
        }


        public async Task<int> GetLoyaltyPoints(int userId)
        {
            var user = await _libraryContext.Users
           .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return -1;
            }

            return user.Points;
        }
    

        private async Task<bool> checkUserIfExist(int userId)
        {
            var userExist = await _libraryContext.Users.FindAsync(userId);
            return userExist != null;
        }
    }
}
