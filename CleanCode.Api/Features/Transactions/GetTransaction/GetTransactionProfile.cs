using AutoMapper;

namespace CleanCode.Api.Features.Transactions.GetTransaction
{
    /// <summary>
    /// Profile do AutoMapper para GetTransaction
    /// </summary>
    public class GetTransactionProfile : Profile
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public GetTransactionProfile()
        {
            CreateMap<GetTransactionRequest, Application.Transactions.GetTransaction.GetTransactionCommand>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<Application.Transactions.GetTransaction.GetTransactionResult, GetTransactionResponse>();
        }
    }
}
