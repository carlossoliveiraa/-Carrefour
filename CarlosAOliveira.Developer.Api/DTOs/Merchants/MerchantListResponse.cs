using CarlosAOliveira.Developer.Application.DTOs.Base;

namespace CarlosAOliveira.Developer.Api.DTOs.Merchants
{
    /// <summary>
    /// Response DTO for paginated merchant list
    /// </summary>
    public class MerchantListResponse : PagedResult<MerchantResponse>
    {
        /// <summary>
        /// Initializes a new instance of MerchantListResponse
        /// </summary>
        public MerchantListResponse() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of MerchantListResponse with data
        /// </summary>
        /// <param name="items">Merchant items</param>
        /// <param name="totalCount">Total count</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        public MerchantListResponse(IEnumerable<MerchantResponse> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
