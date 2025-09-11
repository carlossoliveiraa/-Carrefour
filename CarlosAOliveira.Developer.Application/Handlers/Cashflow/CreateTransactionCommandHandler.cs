using AutoMapper;
using CarlosAOliveira.Developer.Application.Commands.Cashflow;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Cashflow;
using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Domain.Events;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.Cashflow
{
    /// <summary>
    /// Handler for creating a new transaction
    /// </summary>
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, BaseResponse<TransactionResponse>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IEventQueue _eventQueue;
        private readonly IMapper _mapper;

        public CreateTransactionCommandHandler(
            ITransactionRepository transactionRepository,
            IEventQueue eventQueue,
            IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _eventQueue = eventQueue;
            _mapper = mapper;
        }

        public async Task<BaseResponse<TransactionResponse>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate transaction type
                if (!Enum.TryParse<TransactionType>(request.Type, out var transactionType))
                {
                    return BaseResponse<TransactionResponse>.CreateError("Invalid transaction type. Must be 'Credit' or 'Debit'");
                }

                // Create transaction
                var transaction = new Domain.Entities.Transaction(
                    request.Date,
                    request.Amount,
                    transactionType,
                    request.Category,
                    request.Description
                );

                // Save transaction
                await _transactionRepository.AddAsync(transaction, cancellationToken);
                await _transactionRepository.SaveChangesAsync(cancellationToken);

                // Publish event to queue
                var domainEvent = new TransactionCreatedEvent(transaction);
                await _eventQueue.PublishAsync(domainEvent);

                // Map to response
                var response = _mapper.Map<TransactionResponse>(transaction);

                return BaseResponse<TransactionResponse>.CreateSuccess(response, "Transaction created successfully");
            }
            catch (Exception ex)
            {
                return BaseResponse<TransactionResponse>.CreateError($"Error creating transaction: {ex.Message}");
            }
        }
    }
}
