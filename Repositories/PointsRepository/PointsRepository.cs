using Backend.Backend.Repositories.PointsRepository;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Repositories.PointsRepository
{
    public class PointsRepository : IPointsRepository
    {
        private readonly LibraryContext _context;

        public PointsRepository(LibraryContext context)
        {
            _context = context;
        }

        public async Task<List<OperationHistory>> GetOperationHistoryByUserIdAsync(int userId)
        {
            return await _context.OperationHistories
                .Where(o => o.UserId == userId)
                .OrderBy(o => o.DateTime)
                .ToListAsync();
        }

        public async Task addOperationHistory(OperationHistory operationHistory)
        {
            await _context.OperationHistories.AddAsync(operationHistory);
            await _context.SaveChangesAsync();
        }
    }
}
