using MediatR;
using System.Collections.ObjectModel;

namespace V2.Application.Modules.SellerAssistantModule
{
    public class GetSellerAssistant : IRequest<GetSellerAssistantResult>
    {
        public GetSellerAssistant(string sellerId)
        {
            SellerId = sellerId;
        }

        public string SellerId { get; }
    }

    public class GetSellerAssistantResult
    {
        public string UserId { get; set; }
        public string PersonId { get; set; }
        public bool IsEnabled { get; set; }
        public string Email { get; set; }
        public ReadOnlyCollection<string> Permissions { get; set; }
    }
}
