using CleanCode.Common.Validation;
using CleanCode.Domain.Enum;
using MediatR;

namespace CleanCode.Application.Transactions.CreateTransaction
{
    /// <summary>
    /// Command para criar uma nova transação
    /// </summary>
    public class CreateTransactionCommand : IRequest<CreateTransactionResult>
    {
        /// <summary>
        /// Descrição da transação
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Valor da transação
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Tipo da transação (Débito ou Crédito)
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// Data da transação
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Categoria da transação (opcional)
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Observações adicionais (opcional)
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Valida o command
        /// </summary>
        /// <returns>Resultado da validação</returns>
        public ValidationResultDetail Validate()
        {
            var validator = new CreateTransactionCommandValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }
    }
}
