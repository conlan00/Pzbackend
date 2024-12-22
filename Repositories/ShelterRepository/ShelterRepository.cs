using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories.ShelterRepository
{
    public class ShelterRepository : IShelterRepository
    {
        private readonly LibraryContext _libraryContext;

        public ShelterRepository(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        public async Task<IEnumerable<Shelter>> GetAllSheltersAsync()
        {
            return await _libraryContext.Shelters.ToListAsync();
        }

        public Task<IEnumerable<Shelter>> GetNearbySheltersAsync(double userLat, double userLong, double radius)
        {
            throw new NotImplementedException();
        }
    }


}
