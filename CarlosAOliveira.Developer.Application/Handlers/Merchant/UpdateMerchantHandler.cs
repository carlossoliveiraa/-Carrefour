using AutoMapper;
using CarlosAOliveira.Developer.Application.Commands.Merchant;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Merchant;
using CarlosAOliveira.Developer.Application.Handlers.Base;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.Merchant
{
    public class UpdateMerchantHandler : BaseHandler, IRequestHandler<UpdateMerchantCommand, BaseResponse<MerchantDto>>
    {
        private readonly IMerchantRepository _merchantRepository;

        public UpdateMerchantHandler(IMapper mapper, IMerchantRepository merchantRepository) : base(mapper)
        {
            _merchantRepository = merchantRepository;
        }

        public async Task<BaseResponse<MerchantDto>> Handle(UpdateMerchantCommand request, CancellationToken cancellationToken)
        {
            var merchant = await _merchantRepository.GetByIdAsync(request.Id, cancellationToken);
            if (merchant == null)
            {
                return Error<MerchantDto>("Merchant not found");
            }

            merchant.UpdateInformation(request.Name, request.Email);
            await _merchantRepository.UpdateAsync(merchant, cancellationToken);
            await _merchantRepository.SaveChangesAsync(cancellationToken);
            
            var merchantDto = Mapper.Map<MerchantDto>(merchant);
            return Success(merchantDto, "Merchant updated successfully");
        }
    }
}