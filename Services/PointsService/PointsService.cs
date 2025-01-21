using Backend.Backend.Repositories.PointsRepository;
using Backend.DTO;

namespace Backend.Services.PointsService
{
    public class PointsService : IPointsService
    {
        private readonly IPointsRepository _pointsRepository;

        public PointsService(IPointsRepository pointsRepository)
        {
            _pointsRepository = pointsRepository;
        }

        public async Task<List<UserPointsHistoryResponse>> GetUserPointsHistoryAsync(int userId)
        {
            var operationHistory = await _pointsRepository.GetOperationHistoryByUserIdAsync(userId);

            return operationHistory.Select(o => new UserPointsHistoryResponse
            {
                Date = o.DateTime,
                OperationDescription = o.OperationDescription
            }).ToList();
        }
    }
}
