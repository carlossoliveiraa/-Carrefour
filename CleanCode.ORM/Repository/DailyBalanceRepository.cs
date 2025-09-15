using CleanCode.Domain.Entities;
using CleanCode.Domain.Repositories.Interfaces;
using CleanCode.ORM;
using Microsoft.EntityFrameworkCore;

namespace CleanCode.ORM.Repository
{
    /// <summary>
    /// Implementação do repositório de saldos diários
    /// </summary>
    public class DailyBalanceRepository : IDailyBalanceRepository
    {
        private readonly DefaultContext _context;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        public DailyBalanceRepository(DefaultContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria ou atualiza um saldo diário
        /// </summary>
        /// <param name="dailyBalance">Saldo diário</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Saldo diário criado/atualizado</returns>
        public async Task<DailyBalance> CreateOrUpdateAsync(DailyBalance dailyBalance, CancellationToken cancellationToken = default)
        {
            var existing = await _context.DailyBalances
                .FirstOrDefaultAsync(db => db.Date.Date == dailyBalance.Date.Date, cancellationToken);

            if (existing != null)
            {
                // Atualiza o existente
                existing.OpeningBalance = dailyBalance.OpeningBalance;
                existing.TotalCredits = dailyBalance.TotalCredits;
                existing.TotalDebits = dailyBalance.TotalDebits;
                existing.ClosingBalance = dailyBalance.ClosingBalance;
                existing.CreditTransactionCount = dailyBalance.CreditTransactionCount;
                existing.DebitTransactionCount = dailyBalance.DebitTransactionCount;
                existing.TotalTransactionCount = dailyBalance.TotalTransactionCount;
                existing.LastUpdated = DateTime.UtcNow;

                _context.DailyBalances.Update(existing);
                await _context.SaveChangesAsync(cancellationToken);
                return existing;
            }
            else
            {
                // Cria novo
                _context.DailyBalances.Add(dailyBalance);
                await _context.SaveChangesAsync(cancellationToken);
                return dailyBalance;
            }
        }

        /// <summary>
        /// Busca um saldo diário por data
        /// </summary>
        /// <param name="date">Data do saldo</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Saldo diário encontrado ou null</returns>
        public async Task<DailyBalance?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.DailyBalances
                .FirstOrDefaultAsync(db => db.Date.Date == date.Date, cancellationToken);
        }

        /// <summary>
        /// Busca saldos diários por período
        /// </summary>
        /// <param name="startDate">Data inicial</param>
        /// <param name="endDate">Data final</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de saldos diários do período</returns>
        public async Task<IEnumerable<DailyBalance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.DailyBalances
                .Where(db => db.Date.Date >= startDate.Date && db.Date.Date <= endDate.Date)
                .OrderBy(db => db.Date)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Busca o último saldo diário
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Último saldo diário ou null</returns>
        public async Task<DailyBalance?> GetLastAsync(CancellationToken cancellationToken = default)
        {
            return await _context.DailyBalances
                .OrderByDescending(db => db.Date)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Busca o saldo diário mais recente antes de uma data específica
        /// </summary>
        /// <param name="date">Data de referência</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Saldo diário mais recente ou null</returns>
        public async Task<DailyBalance?> GetLastBeforeDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.DailyBalances
                .Where(db => db.Date.Date < date.Date)
                .OrderByDescending(db => db.Date)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Conta o número de saldos diários por período
        /// </summary>
        /// <param name="startDate">Data inicial</param>
        /// <param name="endDate">Data final</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Número de saldos diários</returns>
        public async Task<int> CountByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.DailyBalances
                .CountAsync(db => db.Date.Date >= startDate.Date && db.Date.Date <= endDate.Date, cancellationToken);
        }

        /// <summary>
        /// Verifica se existe saldo diário para uma data
        /// </summary>
        /// <param name="date">Data do saldo</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>True se existe saldo para a data</returns>
        public async Task<bool> ExistsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            return await _context.DailyBalances
                .AnyAsync(db => db.Date.Date == date.Date, cancellationToken);
        }

        /// <summary>
        /// Remove um saldo diário
        /// </summary>
        /// <param name="date">Data do saldo</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>True se removido com sucesso</returns>
        public async Task<bool> DeleteByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var dailyBalance = await _context.DailyBalances
                .FirstOrDefaultAsync(db => db.Date.Date == date.Date, cancellationToken);

            if (dailyBalance == null)
                return false;

            _context.DailyBalances.Remove(dailyBalance);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
