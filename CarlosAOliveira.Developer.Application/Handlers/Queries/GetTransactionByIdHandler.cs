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
    /// Handler for getting transaction by ID
    /// </summary>
    public class GetTransactionByIdHandler : BaseHandler, IRequestHandler<GetTransactionByIdQuery, BaseResponse<TransactionDto>>
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetTransactionByIdHandler(IMapper mapper, ITransactionRepository transactionRepository) : base(mapper)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<BaseResponse<TransactionDto>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var transaction = await _transactionRepository.GetByIdAsync(request.Id);
                if (transaction == null)
                {
                    return Error<TransactionDto>("Transaction not found");
                }

                var transactionDto = Mapper.Map<TransactionDto>(transaction);
                return Success(transactionDto);
            }
            catch (Exception ex)
            {
                return Error<TransactionDto>($"Error retrieving transaction: {ex.Message}");
            }
        }
    }
}
