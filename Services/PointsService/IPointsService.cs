using Backend.DTO;

namespace Backend.Services.PointsService
{
    public interface IPointsService
    {
        Task<List<UserPointsHistoryResponse>> GetUserPointsHistoryAsync(int userId);
    }
}
