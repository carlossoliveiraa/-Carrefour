using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.Transaction;

namespace CarlosAOliveira.Developer.Application.Mappings
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<Domain.Entities.Transaction, TransactionDto>();
        }
    }
}