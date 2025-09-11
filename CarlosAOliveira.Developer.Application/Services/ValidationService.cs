using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Domain.Enums;
using FluentValidation;

namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// Service for centralized validation logic
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Validates a request and returns validation result
        /// </summary>
        /// <typeparam name="T">Type to validate</typeparam>
        /// <param name="request">Request to validate</param>
        /// <returns>Validation result</returns>
        public async Task<BaseResponse> ValidateAsync<T>(T request)
        {
            var validator = _serviceProvider.GetService(typeof(IValidator<T>)) as IValidator<T>;
            if (validator == null)
            {
                return BaseResponse.CreateSuccess();
            }

            var result = await validator.ValidateAsync(request, CancellationToken.None);
            if (!result.IsValid)
            {
                var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                return BaseResponse.CreateError("Validation failed", errors);
            }

            return BaseResponse.CreateSuccess();
        }

        /// <summary>
        /// Validates business rules for a transaction
        /// </summary>
        /// <param name="amount">Transaction amount</param>
        /// <param name="type">Transaction type</param>
        /// <param name="date">Transaction date</param>
        /// <returns>Validation result</returns>
        public Task<BaseResponse> ValidateTransactionAsync(decimal amount, string type, DateOnly date)
        {
            var errors = new List<string>();

            // Validate amount
            if (amount <= 0)
            {
                errors.Add("Amount must be greater than zero");
            }

            if (amount > 1000000) // Business rule: max transaction amount
            {
                errors.Add("Amount cannot exceed 1,000,000");
            }

            // Validate transaction type
            if (!Enum.TryParse<TransactionType>(type, true, out _))
            {
                errors.Add("Invalid transaction type. Must be 'Credit' or 'Debit'");
            }

            // Validate date
            if (date > DateOnly.FromDateTime(DateTime.Today))
            {
                errors.Add("Transaction date cannot be in the future");
            }

            if (date < DateOnly.FromDateTime(DateTime.Today.AddYears(-1)))
            {
                errors.Add("Transaction date cannot be older than 1 year");
            }

            if (errors.Any())
            {
                return Task.FromResult(BaseResponse.CreateError("Transaction validation failed", errors));
            }

            return Task.FromResult(BaseResponse.CreateSuccess());
        }
    }
}
