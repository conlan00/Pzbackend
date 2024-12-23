using Backend.Models;

namespace Backend.Repositories.ShelterRepository
{
    public interface IShelterRepository
    {
        Task<IEnumerable<Shelter>> GetAllSheltersAsync();
        Task<IEnumerable<Shelter>> GetNearbySheltersAsync(double userLat, double userLong, double radius);
        Task<IEnumerable<Book>> GetBooksInShelterAsync(int shelterId);
    }
}
