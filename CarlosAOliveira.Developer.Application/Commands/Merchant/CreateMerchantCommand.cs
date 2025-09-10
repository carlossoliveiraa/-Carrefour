using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Merchant;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Commands.Merchant
{
    public class CreateMerchantCommand : IRequest<BaseResponse<MerchantDto>>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}