using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Merchant;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Queries.Merchant
{
    /// <summary>
    /// Query to get a merchant by ID
    /// </summary>
    public class GetMerchantByIdQuery : IRequest<BaseResponse<MerchantDto>>
    {
        public Guid Id { get; set; }

        public GetMerchantByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
