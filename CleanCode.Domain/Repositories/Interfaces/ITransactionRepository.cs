using CleanCode.Domain.Entities;

namespace CleanCode.Domain.Repositories.Interfaces
{
    /// <summary>
    /// Interface para repositório de transações
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Cria uma nova transação
        /// </summary>
        /// <param name="transaction">Transação a ser criada</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Transação criada</returns>
        Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca uma transação por ID
        /// </summary>
        /// <param name="id">ID da transação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Transação encontrada ou null</returns>
        Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca transações por data
        /// </summary>
        /// <param name="date">Data das transações</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de transações do dia</returns>
        Task<IEnumerable<Transaction>> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca transações por período
        /// </summary>
        /// <param name="startDate">Data inicial</param>
        /// <param name="endDate">Data final</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de transações do período</returns>
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca transações por tipo
        /// </summary>
        /// <param name="type">Tipo da transação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de transações do tipo</returns>
        Task<IEnumerable<Transaction>> GetByTypeAsync(Domain.Enum.TransactionType type, CancellationToken cancellationToken = default);

        /// <summary>
        /// Atualiza uma transação
        /// </summary>
        /// <param name="transaction">Transação a ser atualizada</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Transação atualizada</returns>
        Task<Transaction> UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove uma transação
        /// </summary>
        /// <param name="id">ID da transação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>True se removida com sucesso</returns>
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Conta o número de transações por data
        /// </summary>
        /// <param name="date">Data das transações</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Número de transações</returns>
        Task<int> CountByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
