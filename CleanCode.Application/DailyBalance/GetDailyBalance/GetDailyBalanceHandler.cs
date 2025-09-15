using AutoMapper;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanCode.Application.DailyBalance.GetDailyBalance
{
    /// <summary>
    /// Handler para buscar saldo diário por data
    /// </summary>
    public class GetDailyBalanceHandler : IRequestHandler<GetDailyBalanceCommand, GetDailyBalanceResult>
    {
        private readonly IDailyBalanceRepository _dailyBalanceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDailyBalanceHandler> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="dailyBalanceRepository">Repositório de saldos diários</param>
        /// <param name="mapper">AutoMapper</param>
        /// <param name="logger">Logger</param>
        public GetDailyBalanceHandler(
            IDailyBalanceRepository dailyBalanceRepository,
            IMapper mapper,
            ILogger<GetDailyBalanceHandler> logger)
        {
            _dailyBalanceRepository = dailyBalanceRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Executa a busca do saldo diário
        /// </summary>
        /// <param name="request">Command de busca</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Resultado da busca</returns>
        public async Task<GetDailyBalanceResult> Handle(GetDailyBalanceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando saldo diário para data: {Date}", request.Date);

            var dailyBalance = await _dailyBalanceRepository.GetByDateAsync(request.Date, cancellationToken);

            if (dailyBalance == null)
            {
                _logger.LogWarning("Saldo diário não encontrado para data: {Date}", request.Date);
                throw new KeyNotFoundException($"Saldo diário para data {request.Date:yyyy-MM-dd} não encontrado");
            }

            _logger.LogInformation("Saldo diário encontrado para data: {Date}", request.Date);

            var result = _mapper.Map<GetDailyBalanceResult>(dailyBalance);
            return result;
        }
    }
}
