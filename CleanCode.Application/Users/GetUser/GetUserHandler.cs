using AutoMapper;
using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging.Models;
using CleanCode.Domain.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanCode.Application.Users.GetUser
{
    
    public class GetUserHandler : IRequestHandler<GetUserCommand, GetUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;
        private readonly ILogger<GetUserHandler> _logger;
              
        public GetUserHandler(
            IUserRepository userRepository,
            IMapper mapper,
            IMessageService messageService,
            ILogger<GetUserHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _messageService = messageService;
            _logger = logger;
        }
               
        public async Task<GetUserResult> Handle(GetUserCommand request, CancellationToken cancellationToken)
        {
            var validator = new GetUserValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {request.Id} not found");

            // Envia mensagem para a fila de usuários acessados
            try
            {
                var userAccessedMessage = new UserAccessedMessage
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    AccessType = "GET",
                    AccessedAt = DateTime.UtcNow
                };

                await _messageService.SendMessageAsync(userAccessedMessage, "user_accessed", cancellationToken);
                _logger.LogInformation("User accessed message sent to queue for user {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send user accessed message for user {UserId}", user.Id);
                // Não falha a operação se a mensageria falhar
            }

            return _mapper.Map<GetUserResult>(user);
        }
    }
}
