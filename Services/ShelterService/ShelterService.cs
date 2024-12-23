using Backend.Models;
using Backend.Repositories.ShelterRepository;

namespace Backend.Services.ShelterService
{
    public class ShelterService : IShelterService
    {
        private readonly IShelterRepository _shelterRepository;

        public ShelterService(IShelterRepository shelterRepository)
        {
            _shelterRepository = shelterRepository;
        }

        public async Task<IEnumerable<Shelter>> GetAllSheltersAsync()
        {
            return await _shelterRepository.GetAllSheltersAsync();
        }

        public async Task<IEnumerable<Shelter>> GetNearbySheltersAsync(double userLat, double userLong, double radius)
        {
            // Pobierz wszystkie schroniska z repozytorium
            var shelters = await _shelterRepository.GetAllSheltersAsync();
            foreach (var shelter in shelters)
            {
                Console.WriteLine($"ID: {shelter.Id}, Name: {shelter.Name}, Latitude: {shelter.Lat}, Longitude: {shelter.Long}");
            }
            // Filtruj schroniska na podstawie odleg³oœci
            return shelters.Where(shelter =>
            {
                double distance = CalculateDistance(userLat, userLong, shelter.Lat, shelter.Long);

                Console.WriteLine(distance);
                return distance <= radius;
            }).ToList();
        }

        // Funkcja obliczaj¹ca odleg³oœæ miêdzy dwoma punktami za pomoc¹ wzoru Haversine
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371;

            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
