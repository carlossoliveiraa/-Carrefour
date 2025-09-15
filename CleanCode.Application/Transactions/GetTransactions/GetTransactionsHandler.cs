using AutoMapper;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanCode.Application.Transactions.GetTransactions
{
    /// <summary>
    /// Handler para buscar transações com filtros
    /// </summary>
    public class GetTransactionsHandler : IRequestHandler<GetTransactionsCommand, GetTransactionsResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTransactionsHandler> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="transactionRepository">Repositório de transações</param>
        /// <param name="mapper">AutoMapper</param>
        /// <param name="logger">Logger</param>
        public GetTransactionsHandler(
            ITransactionRepository transactionRepository,
            IMapper mapper,
            ILogger<GetTransactionsHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Executa a busca das transações
        /// </summary>
        /// <param name="request">Command de busca</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Resultado da busca</returns>
        public async Task<GetTransactionsResult> Handle(GetTransactionsCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando transações com filtros - Data inicial: {StartDate}, Data final: {EndDate}, Tipo: {Type}, Categoria: {Category}", 
                request.StartDate, request.EndDate, request.Type, request.Category);

            // Busca transações baseado nos filtros
            IEnumerable<Transaction> allTransactions;

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                allTransactions = await _transactionRepository.GetByDateRangeAsync(request.StartDate.Value, request.EndDate.Value, cancellationToken);
            }
            else if (request.StartDate.HasValue)
            {
                allTransactions = await _transactionRepository.GetByDateAsync(request.StartDate.Value, cancellationToken);
            }
            else if (request.EndDate.HasValue)
            {
                allTransactions = await _transactionRepository.GetByDateAsync(request.EndDate.Value, cancellationToken);
            }
            else
            {
                // Se não há filtro de data, busca por um período amplo (últimos 30 dias)
                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddDays(-30);
                allTransactions = await _transactionRepository.GetByDateRangeAsync(startDate, endDate, cancellationToken);
            }

            var transactionList = allTransactions.ToList();

            // Aplica filtros
            var filteredTransactions = transactionList.AsQueryable();

            if (request.StartDate.HasValue)
            {
                filteredTransactions = filteredTransactions.Where(t => t.TransactionDate.Date >= request.StartDate.Value.Date);
            }

            if (request.EndDate.HasValue)
            {
                filteredTransactions = filteredTransactions.Where(t => t.TransactionDate.Date <= request.EndDate.Value.Date);
            }

            if (request.Type.HasValue && request.Type.Value != TransactionType.None)
            {
                filteredTransactions = filteredTransactions.Where(t => t.Type == request.Type.Value);
            }

            if (!string.IsNullOrEmpty(request.Category))
            {
                filteredTransactions = filteredTransactions.Where(t => t.Category == request.Category);
            }

            var totalCount = filteredTransactions.Count();

            // Aplica paginação
            var pagedTransactions = filteredTransactions
                .OrderByDescending(t => t.TransactionDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            _logger.LogInformation("Encontradas {Count} transações de {Total} total", pagedTransactions.Count, totalCount);

            // Mapeia os resultados
            var transactionItems = _mapper.Map<List<GetTransactionsResult.TransactionItem>>(pagedTransactions);

            var result = new GetTransactionsResult
            {
                Transactions = transactionItems,
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };

            return result;
        }
    }
}
