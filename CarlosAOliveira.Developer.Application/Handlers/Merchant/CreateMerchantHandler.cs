using AutoMapper;
using CarlosAOliveira.Developer.Application.Commands.Merchant;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Merchant;
using CarlosAOliveira.Developer.Application.Handlers.Base;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.Merchant
{
    public class CreateMerchantHandler : BaseHandler, IRequestHandler<CreateMerchantCommand, BaseResponse<MerchantDto>>
    {
        private readonly IMerchantRepository _merchantRepository;

        public CreateMerchantHandler(IMapper mapper, IMerchantRepository merchantRepository) : base(mapper)
        {
            _merchantRepository = merchantRepository;
        }

        public async Task<BaseResponse<MerchantDto>> Handle(CreateMerchantCommand request, CancellationToken cancellationToken)
        {
            var merchant = new Domain.Entities.Merchant(request.Name, request.Email);
            await _merchantRepository.AddAsync(merchant);
            await _merchantRepository.SaveChangesAsync();
            
            var merchantDto = Mapper.Map<MerchantDto>(merchant);
            return Success(merchantDto, "Merchant created successfully");
        }
    }
}