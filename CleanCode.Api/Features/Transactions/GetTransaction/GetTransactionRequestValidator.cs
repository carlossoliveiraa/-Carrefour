using FluentValidation;

namespace CleanCode.Api.Features.Transactions.GetTransaction
{
    /// <summary>
    /// Validador para GetTransactionRequest
    /// </summary>
    public class GetTransactionRequestValidator : AbstractValidator<GetTransactionRequest>
    {
        /// <summary>
        /// Construtor com regras de validação
        /// </summary>
        public GetTransactionRequestValidator()
        {
            RuleFor(request => request.Id)
                .NotEmpty()
                .WithMessage("O ID da transação é obrigatório");
        }
    }
}
