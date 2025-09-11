using AutoMapper;
using CarlosAOliveira.Developer.Application.Commands.User;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.User;
using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CarlosAOliveira.Developer.Application.Handlers.User
{
    /// <summary>
    /// Handler for creating a new user
    /// </summary>
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, BaseResponse<UserDto>>
    {
        private readonly UserManager<Domain.Entities.User> _userManager;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(UserManager<Domain.Entities.User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<BaseResponse<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BaseResponse<UserDto>.CreateError("Já existe um usuário com este email");
                }

                // Create new user
                var user = new Domain.Entities.User
                {
                    Id = Guid.NewGuid(),
                    UserName = request.Username,
                    NormalizedUserName = request.Username.ToUpperInvariant(),
                    Email = request.Email,
                    NormalizedEmail = request.Email.ToUpperInvariant(),
                    EmailConfirmed = true, // For development, skip email confirmation
                    PhoneNumber = request.Phone ?? string.Empty,
                    PhoneNumberConfirmed = true, // For development, skip phone confirmation
                    LockoutEnabled = true, // Enable lockout for security
                    LockoutEnd = null, // No lockout initially
                    AccessFailedCount = 0, // Initialize access failed count
                    TwoFactorEnabled = false, // Disable two factor for development
                    SecurityStamp = Guid.NewGuid().ToString(), // Generate security stamp
                    ConcurrencyStamp = Guid.NewGuid().ToString(), // Generate concurrency stamp
                    Role = UserRole.Customer, // Default role
                    Status = UserStatus.Active, // Default status
                    CreatedAt = DateTime.UtcNow
                };

                // Create user with password (Identity will hash it automatically)
                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BaseResponse<UserDto>.CreateError($"Erro ao criar usuário: {errors}");
                }

                // Map to DTO
                var userDto = _mapper.Map<UserDto>(user);

                return BaseResponse<UserDto>.CreateSuccess(userDto, "Usuário criado com sucesso");
            }
            catch (Exception ex)
            {
                return BaseResponse<UserDto>.CreateError($"Erro interno: {ex.Message}");
            }
        }
    }
}
