using AutoMapper;
using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging.Models;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanCode.Application.Transactions.UpdateTransaction
{
    /// <summary>
    /// Handler para atualizar uma transação existente
    /// </summary>
    public class UpdateTransactionHandler : IRequestHandler<UpdateTransactionCommand, UpdateTransactionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateTransactionHandler> _logger;
        private readonly IMessageService _messageService;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="transactionRepository">Repositório de transações</param>
        /// <param name="mapper">AutoMapper</param>
        /// <param name="logger">Logger</param>
        /// <param name="messageService">Serviço de mensageria</param>
        public UpdateTransactionHandler(
            ITransactionRepository transactionRepository,
            IMapper mapper,
            ILogger<UpdateTransactionHandler> logger,
            IMessageService messageService)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _logger = logger;
            _messageService = messageService;
        }

        /// <summary>
        /// Executa a atualização da transação
        /// </summary>
        /// <param name="request">Command de atualização</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Resultado da atualização</returns>
        public async Task<UpdateTransactionResult> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando atualização de transação: {Id}", request.Id);

            // Valida o command
            var validator = new UpdateTransactionCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validação falhou para atualização de transação: {Id}", request.Id);
                throw new ValidationException(validationResult.Errors);
            }

            // Busca a transação existente
            var existingTransaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingTransaction == null)
            {
                _logger.LogWarning("Transação não encontrada para atualização: {Id}", request.Id);
                throw new KeyNotFoundException($"Transação com ID {request.Id} não encontrada");
            }

            // Atualiza as propriedades
            existingTransaction.Description = request.Description;
            existingTransaction.Amount = request.Amount;
            existingTransaction.Type = request.Type;
            existingTransaction.TransactionDate = request.TransactionDate;
            existingTransaction.Category = request.Category;
            existingTransaction.Notes = request.Notes;

            // Salva a transação atualizada
            var updatedTransaction = await _transactionRepository.UpdateAsync(existingTransaction, cancellationToken);

            _logger.LogInformation("Transação atualizada com sucesso: {Id}", updatedTransaction.Id);

            // Envia mensagem para a fila de transações atualizadas
            try
            {
                var transactionUpdatedMessage = new TransactionUpdatedMessage
                {
                    TransactionId = updatedTransaction.Id,
                    Description = updatedTransaction.Description,
                    Amount = updatedTransaction.Amount,
                    Type = updatedTransaction.Type.ToString(),
                    TransactionDate = updatedTransaction.TransactionDate,
                    Category = updatedTransaction.Category,
                    Notes = updatedTransaction.Notes,
                    UpdatedAt = DateTime.UtcNow
                };

                await _messageService.SendMessageAsync(transactionUpdatedMessage, "transaction_updated", cancellationToken);
                _logger.LogInformation("Mensagem de transação atualizada enviada para a fila: {TransactionId}", updatedTransaction.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem de transação atualizada: {TransactionId}", updatedTransaction.Id);
                // Não falha a operação se a mensagem não for enviada
            }

            // Mapeia o resultado
            var result = _mapper.Map<UpdateTransactionResult>(updatedTransaction);
            return result;
        }
    }
}
