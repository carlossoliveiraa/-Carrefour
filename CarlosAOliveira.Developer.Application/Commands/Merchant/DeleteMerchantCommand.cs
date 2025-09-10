using MediatR;

namespace CarlosAOliveira.Developer.Application.Commands.Merchant
{
    /// <summary>
    /// Command to delete a merchant
    /// </summary>
    public class DeleteMerchantCommand : IRequest<bool>
    {
        /// <summary>
        /// Merchant ID to delete
        /// </summary>
        public Guid Id { get; set; }
    }
}
