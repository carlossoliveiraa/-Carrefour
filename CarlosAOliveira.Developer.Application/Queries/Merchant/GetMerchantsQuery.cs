using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Merchant;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Queries.Merchant
{
    /// <summary>
    /// Query to get paginated list of merchants
    /// </summary>
    public class GetMerchantsQuery : IRequest<PagedResult<MerchantDto>>
    {
        /// <summary>
        /// Page number (1-based)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
