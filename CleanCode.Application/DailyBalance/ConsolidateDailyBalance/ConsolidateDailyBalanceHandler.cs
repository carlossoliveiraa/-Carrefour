using AutoMapper;
using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging.Models;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using CleanCode.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanCode.Application.DailyBalance.ConsolidateDailyBalance
{
    /// <summary>
    /// Handler para consolidar o saldo diário
    /// </summary>
    public class ConsolidateDailyBalanceHandler : IRequestHandler<ConsolidateDailyBalanceCommand, ConsolidateDailyBalanceResult>
    {
        private readonly IDailyBalanceRepository _dailyBalanceRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ConsolidateDailyBalanceHandler> _logger;
        private readonly IMessageService _messageService;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="dailyBalanceRepository">Repositório de saldos diários</param>
        /// <param name="transactionRepository">Repositório de transações</param>
        /// <param name="mapper">AutoMapper</param>
        /// <param name="logger">Logger</param>
        /// <param name="messageService">Serviço de mensageria</param>
        public ConsolidateDailyBalanceHandler(
            IDailyBalanceRepository dailyBalanceRepository,
            ITransactionRepository transactionRepository,
            IMapper mapper,
            ILogger<ConsolidateDailyBalanceHandler> logger,
            IMessageService messageService)
        {
            _dailyBalanceRepository = dailyBalanceRepository;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _logger = logger;
            _messageService = messageService;
        }

        /// <summary>
        /// Executa a consolidação do saldo diário
        /// </summary>
        /// <param name="request">Command de consolidação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Resultado da consolidação</returns>
        public async Task<ConsolidateDailyBalanceResult> Handle(ConsolidateDailyBalanceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando consolidação do saldo diário para data: {Date}", request.Date);

            // Busca transações do dia
            var transactions = await _transactionRepository.GetByDateAsync(request.Date, cancellationToken);
            var transactionList = transactions.ToList();

            _logger.LogInformation("Encontradas {Count} transações para data: {Date}", transactionList.Count, request.Date);

            // Busca saldo existente ou cria novo
            var existingBalance = await _dailyBalanceRepository.GetByDateAsync(request.Date, cancellationToken);
            var wasCreated = existingBalance == null;

            CleanCode.Domain.Entities.DailyBalance dailyBalance;

            if (existingBalance != null)
            {
                // Atualiza saldo existente
                dailyBalance = existingBalance;
                _logger.LogInformation("Atualizando saldo existente para data: {Date}", request.Date);
            }
            else
            {
                // Busca o último saldo para usar como saldo inicial
                var lastBalance = await _dailyBalanceRepository.GetLastAsync(cancellationToken);
                var openingBalance = lastBalance?.ClosingBalance ?? 0;

                // Cria novo saldo
                dailyBalance = new CleanCode.Domain.Entities.DailyBalance(request.Date, openingBalance);
                _logger.LogInformation("Criando novo saldo para data: {Date} com saldo inicial: {OpeningBalance}", request.Date, openingBalance);
            }

            // Zera os totais para recalcular
            dailyBalance.TotalCredits = 0;
            dailyBalance.TotalDebits = 0;
            dailyBalance.CreditTransactionCount = 0;
            dailyBalance.DebitTransactionCount = 0;
            dailyBalance.TotalTransactionCount = 0;

            // Processa as transações
            foreach (var transaction in transactionList)
            {
                if (transaction.Type == TransactionType.Credit)
                {
                    dailyBalance.AddCredit(transaction.Amount);
                }
                else if (transaction.Type == TransactionType.Debit)
                {
                    dailyBalance.AddDebit(transaction.Amount);
                }
            }

            // Salva o saldo consolidado
            var savedBalance = await _dailyBalanceRepository.CreateOrUpdateAsync(dailyBalance, cancellationToken);

            _logger.LogInformation("Saldo diário consolidado com sucesso para data: {Date}. Créditos: {Credits}, Débitos: {Debits}, Saldo Final: {ClosingBalance}", 
                request.Date, dailyBalance.TotalCredits, dailyBalance.TotalDebits, dailyBalance.ClosingBalance);

            // Envia mensagem para a fila de saldos consolidados
            try
            {
                var dailyBalanceConsolidatedMessage = new DailyBalanceConsolidatedMessage
                {
                    DailyBalanceId = savedBalance.Id,
                    Date = savedBalance.Date,
                    OpeningBalance = savedBalance.OpeningBalance,
                    TotalCredits = savedBalance.TotalCredits,
                    TotalDebits = savedBalance.TotalDebits,
                    ClosingBalance = savedBalance.ClosingBalance,
                    CreditTransactionCount = savedBalance.CreditTransactionCount,
                    DebitTransactionCount = savedBalance.DebitTransactionCount,
                    TotalTransactionCount = savedBalance.TotalTransactionCount,
                    WasCreated = wasCreated,
                    ConsolidatedAt = DateTime.UtcNow
                };

                await _messageService.SendMessageAsync(dailyBalanceConsolidatedMessage, "daily_balance_consolidated", cancellationToken);
                _logger.LogInformation("Mensagem de saldo consolidado enviada para a fila: {DailyBalanceId}", savedBalance.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem de saldo consolidado: {DailyBalanceId}", savedBalance.Id);
                // Não falha a operação se a mensagem não for enviada
            }

            // Mapeia o resultado
            var result = _mapper.Map<ConsolidateDailyBalanceResult>(savedBalance);
            result.WasCreated = wasCreated;

            return result;
        }
    }
}
