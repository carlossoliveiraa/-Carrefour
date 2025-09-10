using AutoMapper;
using CarlosAOliveira.Developer.Application.DTOs.Base;
using CarlosAOliveira.Developer.Application.DTOs.Merchant;
using CarlosAOliveira.Developer.Application.Handlers.Base;
using CarlosAOliveira.Developer.Application.Queries.Merchant;
using CarlosAOliveira.Developer.Domain.Repositories;
using MediatR;

namespace CarlosAOliveira.Developer.Application.Handlers.Queries
{
    /// <summary>
    /// Handler for getting merchant by ID
    /// </summary>
    public class GetMerchantByIdHandler : BaseHandler, IRequestHandler<GetMerchantByIdQuery, BaseResponse<MerchantDto>>
    {
        private readonly IMerchantRepository _merchantRepository;

        public GetMerchantByIdHandler(IMapper mapper, IMerchantRepository merchantRepository) : base(mapper)
        {
            _merchantRepository = merchantRepository;
        }

        public async Task<BaseResponse<MerchantDto>> Handle(GetMerchantByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var merchant = await _merchantRepository.GetByIdAsync(request.Id, cancellationToken);
                if (merchant == null)
                {
                    return Error<MerchantDto>("Merchant not found");
                }

                var merchantDto = Mapper.Map<MerchantDto>(merchant);
                return Success(merchantDto);
            }
            catch (Exception ex)
            {
                return Error<MerchantDto>($"Error retrieving merchant: {ex.Message}");
            }
        }
    }
}
