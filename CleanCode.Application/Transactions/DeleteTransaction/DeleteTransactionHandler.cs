using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging.Models;
using CleanCode.Domain.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanCode.Application.Transactions.DeleteTransaction
{
    /// <summary>
    /// Handler para deletar uma transação
    /// </summary>
    public class DeleteTransactionHandler : IRequestHandler<DeleteTransactionCommand, DeleteTransactionResult>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<DeleteTransactionHandler> _logger;
        private readonly IMessageService _messageService;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="transactionRepository">Repositório de transações</param>
        /// <param name="logger">Logger</param>
        /// <param name="messageService">Serviço de mensageria</param>
        public DeleteTransactionHandler(
            ITransactionRepository transactionRepository,
            ILogger<DeleteTransactionHandler> logger,
            IMessageService messageService)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
            _messageService = messageService;
        }

        /// <summary>
        /// Executa a exclusão da transação
        /// </summary>
        /// <param name="request">Command de exclusão</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Resultado da exclusão</returns>
        public async Task<DeleteTransactionResult> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando exclusão de transação: {Id}", request.Id);

            // Verifica se a transação existe
            var existingTransaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);

            if (existingTransaction == null)
            {
                _logger.LogWarning("Transação não encontrada para exclusão: {Id}", request.Id);
                return new DeleteTransactionResult
                {
                    Id = request.Id,
                    Success = false,
                    Message = $"Transação com ID {request.Id} não encontrada"
                };
            }

            // Deleta a transação
            await _transactionRepository.DeleteAsync(request.Id, cancellationToken);

            _logger.LogInformation("Transação deletada com sucesso: {Id}", request.Id);

            // Envia mensagem para a fila de transações deletadas
            try
            {
                var transactionDeletedMessage = new TransactionDeletedMessage
                {
                    TransactionId = existingTransaction.Id,
                    Description = existingTransaction.Description,
                    Amount = existingTransaction.Amount,
                    Type = existingTransaction.Type.ToString(),
                    TransactionDate = existingTransaction.TransactionDate,
                    Category = existingTransaction.Category,
                    DeletedAt = DateTime.UtcNow
                };

                await _messageService.SendMessageAsync(transactionDeletedMessage, "transaction_deleted", cancellationToken);
                _logger.LogInformation("Mensagem de transação deletada enviada para a fila: {TransactionId}", existingTransaction.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem de transação deletada: {TransactionId}", existingTransaction.Id);
                // Não falha a operação se a mensagem não for enviada
            }

            return new DeleteTransactionResult
            {
                Id = request.Id,
                Success = true,
                Message = "Transação deletada com sucesso"
            };
        }
    }
}
