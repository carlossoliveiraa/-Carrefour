using AutoMapper;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanCode.Application.Transactions.GetTransaction
{
    /// <summary>
    /// Handler para buscar uma transação por ID
    /// </summary>
    public class GetTransactionHandler : IRequestHandler<GetTransactionCommand, GetTransactionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransactionHandler> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="transactionRepository">Repositório de transações</param>
        /// <param name="mapper">AutoMapper</param>
        /// <param name="logger">Logger</param>
        public GetTransactionHandler(
            ITransactionRepository transactionRepository,
            IMapper mapper,
            ILogger<GetTransactionHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Executa a busca da transação
        /// </summary>
        /// <param name="request">Command de busca</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Resultado da busca</returns>
        public async Task<GetTransactionResult> Handle(GetTransactionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando transação: {Id}", request.Id);

            var transaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (transaction == null)
            {
                _logger.LogWarning("Transação não encontrada: {Id}", request.Id);
                throw new KeyNotFoundException($"Transação com ID {request.Id} não encontrada");
            }

            _logger.LogInformation("Transação encontrada: {Id}", request.Id);

            var result = _mapper.Map<GetTransactionResult>(transaction);
            return result;
        }
    }
}
