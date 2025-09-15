using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Repositories.Interfaces;
using CleanCode.ORM;
using Microsoft.EntityFrameworkCore;

namespace CleanCode.ORM.Repository
{
    /// <summary>
    /// Implementação do repositório de transações
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DefaultContext _context;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        public TransactionRepository(DefaultContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria uma nova transação
        /// </summary>
        /// <param name="transaction">Transação a ser criada</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Transação criada</returns>
        public async Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);
            return transaction;
        }

        /// <summary>
        /// Busca uma transação por ID
        /// </summary>
        /// <param name="id">ID da transação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Transação encontrada ou null</returns>
        public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        /// <summary>
        /// Busca transações por data
        /// </summary>
        /// <param name="date">Data das transações</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de transações do dia</returns>
        public async Task<IEnumerable<Transaction>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.Transactions
                .Where(t => t.TransactionDate >= startOfDay && t.TransactionDate < endOfDay)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Busca transações por período
        /// </summary>
        /// <param name="startDate">Data inicial</param>
        /// <param name="endDate">Data final</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de transações do período</returns>
        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var startOfDay = startDate.Date;
            var endOfDay = endDate.Date.AddDays(1);

            return await _context.Transactions
                .Where(t => t.TransactionDate >= startOfDay && t.TransactionDate < endOfDay)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Busca transações por tipo
        /// </summary>
        /// <param name="type">Tipo da transação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de transações do tipo</returns>
        public async Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType type, CancellationToken cancellationToken = default)
        {
            return await _context.Transactions
                .Where(t => t.Type == type)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Atualiza uma transação
        /// </summary>
        /// <param name="transaction">Transação a ser atualizada</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Transação atualizada</returns>
        public async Task<Transaction> UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync(cancellationToken);
            return transaction;
        }

        /// <summary>
        /// Remove uma transação
        /// </summary>
        /// <param name="id">ID da transação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>True se removida com sucesso</returns>
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
                return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        /// <summary>
        /// Conta o número de transações por data
        /// </summary>
        /// <param name="date">Data das transações</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Número de transações</returns>
        public async Task<int> CountByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.Transactions
                .CountAsync(t => t.TransactionDate >= startOfDay && t.TransactionDate < endOfDay, cancellationToken);
        }
    }
}
