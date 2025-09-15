using AutoMapper;
using CleanCode.Api.Common;
using CleanCode.Api.Features.Transactions.CreateTransaction;
using CleanCode.Api.Features.Transactions.GetTransaction;
using CleanCode.Api.Features.Transactions.GetTransactions;
using CleanCode.Application.Transactions.CreateTransaction;
using CleanCode.Application.Transactions.GetTransaction;
using CleanCode.Application.Transactions.GetTransactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanCode.Api.Features.Transactions
{
    /// <summary>
    /// Controller para operações com transações
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="mediator">MediatR</param>
        /// <param name="mapper">AutoMapper</param>
        public TransactionsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria uma nova transação
        /// </summary>
        /// <param name="request">Dados da transação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Transação criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<CreateTransactionResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateTransactionRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<CreateTransactionCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            var response = _mapper.Map<CreateTransactionResponse>(result);
            return CreatedAtAction(nameof(GetTransaction), new { id = result.Id }, new ApiResponseWithData<CreateTransactionResponse> { Data = response, Success = true });
        }

        /// <summary>
        /// Busca uma transação por ID
        /// </summary>
        /// <param name="id">ID da transação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Transação encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<GetTransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTransaction([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new GetTransactionRequest { Id = id };
            var validator = new GetTransactionRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<GetTransactionCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(_mapper.Map<GetTransactionResponse>(result));
        }

        /// <summary>
        /// Lista transações com filtros
        /// </summary>
        /// <param name="request">Filtros de busca</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de transações</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithData<GetTransactionsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTransactions([FromQuery] GetTransactionsRequest request, CancellationToken cancellationToken)
        {
            var validator = new GetTransactionsRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<GetTransactionsCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(_mapper.Map<GetTransactionsResponse>(result));
        }
    }
}
