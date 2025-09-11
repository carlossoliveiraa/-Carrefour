using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarlosAOliveira.Developer.ORM.Repositories
{
    /// <summary>
    /// Daily balance repository implementation
    /// </summary>
    public class DailyBalanceRepository : IDailyBalanceRepository
    {
        private readonly DefaultContext _context;

        public DailyBalanceRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<DailyBalance?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
        {
            return await _context.DailyBalances
                .FirstOrDefaultAsync(db => db.Date == date, cancellationToken);
        }

        public async Task<IEnumerable<DailyBalance>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
        {
            return await _context.DailyBalances
                .Where(db => db.Date >= startDate && db.Date <= endDate)
                .OrderBy(db => db.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<DailyBalance> AddAsync(DailyBalance dailyBalance, CancellationToken cancellationToken = default)
        {
            _context.DailyBalances.Add(dailyBalance);
            await _context.SaveChangesAsync(cancellationToken);
            return dailyBalance;
        }

        public async Task<DailyBalance> UpdateAsync(DailyBalance dailyBalance, CancellationToken cancellationToken = default)
        {
            _context.DailyBalances.Update(dailyBalance);
            await _context.SaveChangesAsync(cancellationToken);
            return dailyBalance;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
