using AutoMapper;
using CarlosAOliveira.Developer.Application.Commands.User;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.User;
using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.User
{
    /// <summary>
    /// Handler for creating a new user
    /// </summary>
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, BaseResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<BaseResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userRepository.GetByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BaseResponse<UserDto>.CreateError("User with this email already exists");
                }

                // Create new user
                var user = new Domain.Entities.User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password, // In production, hash this password
                    Phone = request.Phone ?? string.Empty,
                    Role = UserRole.Customer, // Default role
                    Status = UserStatus.Active, // Default status
                    CreatedAt = DateTime.UtcNow
                };

                // Validate user
                var validationResult = user.Validate();
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.Detail).ToList();
                    return BaseResponse<UserDto>.CreateError("Validation failed", errors);
                }

                // Save user
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                // Map to DTO
                var userDto = _mapper.Map<UserDto>(user);

                return BaseResponse<UserDto>.CreateSuccess(userDto, "User created successfully");
            }
            catch (Exception ex)
            {
                return BaseResponse<UserDto>.CreateError($"Error creating user: {ex.Message}");
            }
        }
    }
}
