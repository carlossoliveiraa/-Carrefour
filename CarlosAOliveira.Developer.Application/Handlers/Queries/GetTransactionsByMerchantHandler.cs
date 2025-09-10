using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Transaction;
using CarlosAOliveira.Developer.Application.Handlers.Base;
using CarlosAOliveira.Developer.Application.Queries.Transaction;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.Queries
{
    /// <summary>
    /// Handler for getting transactions by merchant
    /// </summary>
    public class GetTransactionsByMerchantHandler : BaseHandler, IRequestHandler<GetTransactionsByMerchantQuery, BaseResponse<List<TransactionDto>>>
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionsByMerchantHandler(IMapper mapper, ITransactionRepository transactionRepository) : base(mapper)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<BaseResponse<List<TransactionDto>>> Handle(GetTransactionsByMerchantQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Domain.Entities.Transaction> transactions;

                if (request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    transactions = await _transactionRepository.GetByMerchantIdAndDateRangeAsync(
                        request.MerchantId, 
                        request.StartDate.Value, 
                        request.EndDate.Value, 
                        cancellationToken);
                }
                else
                {
                    transactions = await _transactionRepository.GetByMerchantIdAsync(request.MerchantId, cancellationToken);
                }

                // Apply pagination
                var pagedTransactions = transactions
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var transactionDtos = Mapper.Map<List<TransactionDto>>(pagedTransactions);
                return Success(transactionDtos);
            }
            catch (Exception ex)
            {
                return Error<List<TransactionDto>>($"Error retrieving transactions: {ex.Message}");
            }
        }
    }
}
