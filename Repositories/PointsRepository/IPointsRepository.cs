using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Backend.Repositories.PointsRepository
{
    public interface IPointsRepository
    {
        Task<List<OperationHistory>> GetOperationHistoryByUserIdAsync(int userId);
        Task addOperationHistory(OperationHistory operationHistory);
    }
}