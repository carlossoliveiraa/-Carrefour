using AutoMapper;
using CleanCode.Api.Common;
using CleanCode.Api.Features.DailyBalance.ConsolidateDailyBalance;
using CleanCode.Api.Features.DailyBalance.GetDailyBalance;
using CleanCode.Application.DailyBalance.ConsolidateDailyBalance;
using CleanCode.Application.DailyBalance.GetDailyBalance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanCode.Api.Features.DailyBalance
{
    /// <summary>
    /// Controller para operações com saldo diário
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DailyBalanceController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="mediator">MediatR</param>
        /// <param name="mapper">AutoMapper</param>
        public DailyBalanceController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Busca saldo diário por data
        /// </summary>
        /// <param name="date">Data do saldo</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Saldo diário encontrado</returns>
        [HttpGet("{date:datetime}")]
        [ProducesResponseType(typeof(ApiResponseWithData<GetDailyBalanceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDailyBalance([FromRoute] DateTime date, CancellationToken cancellationToken)
        {
            var request = new GetDailyBalanceRequest { Date = date.Date };
            var validator = new GetDailyBalanceRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<GetDailyBalanceCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(_mapper.Map<GetDailyBalanceResponse>(result));
        }

        /// <summary>
        /// Consolida o saldo diário para uma data específica
        /// </summary>
        /// <param name="request">Dados para consolidação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Saldo consolidado</returns>
        [HttpPost("consolidate")]
        [ProducesResponseType(typeof(ApiResponseWithData<ConsolidateDailyBalanceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConsolidateDailyBalance([FromBody] ConsolidateDailyBalanceRequest request, CancellationToken cancellationToken)
        {
            var validator = new ConsolidateDailyBalanceRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<ConsolidateDailyBalanceCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(_mapper.Map<ConsolidateDailyBalanceResponse>(result));
        }
    }
}
