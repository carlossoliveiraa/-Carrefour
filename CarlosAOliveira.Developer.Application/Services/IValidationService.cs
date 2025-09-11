using CarlosAOliveira.Developer.Application.DTOs.Base;

namespace CarlosAOliveira.Developer.Application.Services
{
    /// <summary>
    /// Service for centralized validation logic
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validates a request and returns validation result
        /// </summary>
        /// <typeparam name="T">Type to validate</typeparam>
        /// <param name="request">Request to validate</param>
        /// <returns>Validation result</returns>
        Task<BaseResponse> ValidateAsync<T>(T request);

        /// <summary>
        /// Validates business rules for a transaction
        /// </summary>
        /// <param name="amount">Transaction amount</param>
        /// <param name="type">Transaction type</param>
        /// <param name="date">Transaction date</param>
        /// <returns>Validation result</returns>
        Task<BaseResponse> ValidateTransactionAsync(decimal amount, string type, DateOnly date);
    }
}
