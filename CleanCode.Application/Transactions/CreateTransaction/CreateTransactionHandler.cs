using AutoMapper;
using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging.Models;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanCode.Application.Transactions.CreateTransaction
{
    /// <summary>
    /// Handler para criar uma nova transação
    /// </summary>
    public class CreateTransactionHandler : IRequestHandler<CreateTransactionCommand, CreateTransactionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateTransactionHandler> _logger;
        private readonly IMessageService _messageService;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="transactionRepository">Repositório de transações</param>
        /// <param name="mapper">AutoMapper</param>
        /// <param name="logger">Logger</param>
        /// <param name="messageService">Serviço de mensageria</param>
        public CreateTransactionHandler(
            ITransactionRepository transactionRepository,
            IMapper mapper,
            ILogger<CreateTransactionHandler> logger,
            IMessageService messageService)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _logger = logger;
            _messageService = messageService;
        }

        /// <summary>
        /// Executa a criação da transação
        /// </summary>
        /// <param name="request">Command de criação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Resultado da criação</returns>
        public async Task<CreateTransactionResult> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando criação de transação: {Description}", request.Description);

            // Valida o command
            var validator = new CreateTransactionCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validação falhou para transação: {Description}", request.Description);
                throw new ValidationException(validationResult.Errors);
            }

            // Mapeia o command para a entidade
            var transaction = _mapper.Map<Transaction>(request);

            // Cria a transação
            var createdTransaction = await _transactionRepository.CreateAsync(transaction, cancellationToken);

            _logger.LogInformation("Transação criada com sucesso: {Id}", createdTransaction.Id);

            // Envia mensagem para a fila de transações criadas
            try
            {
                var transactionCreatedMessage = new TransactionCreatedMessage
                {
                    TransactionId = createdTransaction.Id,
                    Description = createdTransaction.Description,
                    Amount = createdTransaction.Amount,
                    Type = createdTransaction.Type.ToString(),
                    TransactionDate = createdTransaction.TransactionDate,
                    Category = createdTransaction.Category,
                    Notes = createdTransaction.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _messageService.SendMessageAsync(transactionCreatedMessage, "transaction_created", cancellationToken);
                _logger.LogInformation("Mensagem de transação criada enviada para a fila: {TransactionId}", createdTransaction.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem de transação criada: {TransactionId}", createdTransaction.Id);
                // Não falha a operação se a mensagem não for enviada
            }

            // Mapeia o resultado
            var result = _mapper.Map<CreateTransactionResult>(createdTransaction);
            return result;
        }
    }
}
