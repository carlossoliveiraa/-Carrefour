using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;
using CarlosAOliveira.Developer.Application.Queries.Cashflow;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.Cashflow
{
    /// <summary>
    /// Handler for getting a transaction by ID
    /// </summary>
    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, BaseResponse<TransactionResponse>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;

        public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        public async Task<BaseResponse<TransactionResponse>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(request.Id, cancellationToken);
                
                if (transaction == null)
                {
                    return BaseResponse<TransactionResponse>.CreateError("Transaction not found");
                }

                var response = _mapper.Map<TransactionResponse>(transaction);
                return BaseResponse<TransactionResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                return BaseResponse<TransactionResponse>.CreateError($"Error retrieving transaction: {ex.Message}");
            }
        }
    }
}
