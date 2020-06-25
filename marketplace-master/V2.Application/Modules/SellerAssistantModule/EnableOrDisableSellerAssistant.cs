using Framework;
using MediatR;

namespace V2.Application.Modules.SellerAssistantModule
{
    public class EnableOrDisableSellerAssistant : IRequest<OperationResult>
    {
        public EnableOrDisableSellerAssistant(string sellerId, bool enable)
        {
            SellerId = sellerId;
            Enable = enable;
        }

        public string SellerId { get; }
        public bool Enable { get; }
    }
}
