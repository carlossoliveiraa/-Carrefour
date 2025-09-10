using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Transaction;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Queries.Transaction
{
    /// <summary>
    /// Query to get transactions by merchant ID
    /// </summary>
    public class GetTransactionsByMerchantQuery : IRequest<BaseResponse<List<TransactionDto>>>
    {
        public Guid MerchantId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        public GetTransactionsByMerchantQuery(Guid merchantId, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50)
        {
            MerchantId = merchantId;
            StartDate = startDate;
            EndDate = endDate;
            Page = page;
            PageSize = pageSize;
        }
    }
}
