using CleanCode.Domain.Entities;

namespace CleanCode.Domain.Repositories.Interfaces
{
    /// <summary>
    /// Interface para repositório de saldos diários
    /// </summary>
    public interface IDailyBalanceRepository
    {
        /// <summary>
        /// Cria ou atualiza um saldo diário
        /// </summary>
        /// <param name="dailyBalance">Saldo diário</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Saldo diário criado/atualizado</returns>
        Task<DailyBalance> CreateOrUpdateAsync(DailyBalance dailyBalance, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca um saldo diário por data
        /// </summary>
        /// <param name="date">Data do saldo</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Saldo diário encontrado ou null</returns>
        Task<DailyBalance?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca saldos diários por período
        /// </summary>
        /// <param name="startDate">Data inicial</param>
        /// <param name="endDate">Data final</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de saldos diários do período</returns>
        Task<IEnumerable<DailyBalance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Busca o último saldo diário registrado
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Último saldo diário ou null</returns>
        Task<DailyBalance?> GetLastAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica se existe saldo para uma data específica
        /// </summary>
        /// <param name="date">Data a verificar</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>True se existe saldo para a data</returns>
        Task<bool> ExistsByDateAsync(DateTime date, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove um saldo diário
        /// </summary>
        /// <param name="date">Data do saldo</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>True se removido com sucesso</returns>
        Task<bool> DeleteByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
