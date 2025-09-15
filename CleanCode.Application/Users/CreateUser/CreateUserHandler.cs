using AutoMapper;
using CleanCode.Common.Security.Interfaces;
using CleanCode.Common.Messaging.Interfaces;
using CleanCode.Common.Messaging.Models;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Repositories.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanCode.Application.Users.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMessageService _messageService;
        private readonly ILogger<CreateUserHandler> _logger;

        /// <summary>
        /// Initializes a new instance of CreateUserHandler
        /// </summary>
        /// <param name="userRepository">The user repository</param>
        /// <param name="mapper">The AutoMapper instance</param>
        /// <param name="passwordHasher">The password hasher</param>
        /// <param name="messageService">The messaging service</param>
        /// <param name="logger">The logger</param>
        public CreateUserHandler(
            IUserRepository userRepository, 
            IMapper mapper, 
            IPasswordHasher passwordHasher,
            IMessageService messageService,
            ILogger<CreateUserHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _messageService = messageService;
            _logger = logger;
        }

        /// <summary>
        /// Handles the CreateUserCommand request
        /// </summary>
        /// <param name="command">The CreateUser command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created user details</returns>
        public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateUserCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
            if (existingUser != null)
                throw new InvalidOperationException($"User with email {command.Email} already exists");

            var user = _mapper.Map<User>(command);
            user.Password = _passwordHasher.HashPassword(command.Password);

            var createdUser = await _userRepository.CreateAsync(user, cancellationToken);
            
            // Envia mensagem para a fila de usuários criados
            try
            {
                var userCreatedMessage = new UserCreatedMessage
                {
                    UserId = createdUser.Id,
                    Username = createdUser.Username,
                    Email = createdUser.Email,
                    Role = createdUser.Role.ToString(),
                    Status = createdUser.Status.ToString(),
                    CreatedAt = createdUser.CreatedAt
                };

                await _messageService.SendMessageAsync(userCreatedMessage, "user_created", cancellationToken);
                _logger.LogInformation("User created message sent to queue for user {UserId}", createdUser.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send user created message for user {UserId}", createdUser.Id);
                // Não falha a operação se a mensageria falhar
            }

            var result = _mapper.Map<CreateUserResult>(createdUser);
            return result;
        }
    }
}
