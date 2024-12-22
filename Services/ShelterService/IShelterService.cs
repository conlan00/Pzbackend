using Backend.Models;

namespace Backend.Services.ShelterService
{
    public interface IShelterService
    {
        Task<IEnumerable<Shelter>> GetAllSheltersAsync();
        Task<IEnumerable<Shelter>> GetNearbySheltersAsync(double userLat, double userLong, double radius);
    }
}
