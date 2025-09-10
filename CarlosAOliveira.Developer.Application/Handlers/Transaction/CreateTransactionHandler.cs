using AutoMapper;
using CarlosAOliveira.Developer.Application.Commands.Transaction;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Transaction;
using CarlosAOliveira.Developer.Application.Handlers.Base;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.Transaction
{
    public class CreateTransactionHandler : BaseHandler, IRequestHandler<CreateTransactionCommand, BaseResponse<TransactionDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMerchantRepository _merchantRepository;

        public CreateTransactionHandler(
            IMapper mapper,
            ITransactionRepository transactionRepository,
            IMerchantRepository merchantRepository) : base(mapper)
        {
            _transactionRepository = transactionRepository;
            _merchantRepository = merchantRepository;
        }

        public async Task<BaseResponse<TransactionDto>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var merchant = await _merchantRepository.GetByIdAsync(request.MerchantId);
            if (merchant == null)
            {
                return Error<TransactionDto>("Merchant not found");
            }

            var transaction = merchant.CreateTransaction(
                request.Amount,
                request.Type,
                request.Description);

            await _transactionRepository.AddAsync(transaction);
            await _transactionRepository.SaveChangesAsync();
            
            var transactionDto = Mapper.Map<TransactionDto>(transaction);
            return Success(transactionDto, "Transaction created successfully");
        }
    }
}