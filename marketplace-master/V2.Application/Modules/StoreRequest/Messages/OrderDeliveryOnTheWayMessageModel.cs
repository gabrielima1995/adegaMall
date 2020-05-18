using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages
{
    public class OrderDeliveryOnTheWayMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[BUYER_NAME]", DescriptionLabel = "BUYER_NAME_VARIABLE_DESCRIPTION")]
        public string BuyerName { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_ID]", DescriptionLabel = "ORDER_ID_VARIABLE_DESCRIPTION")]
        public string OrderId { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_LINK]", DescriptionLabel = "ORDER_LINK_VARIABLE_DESCRIPTION")]
        public string Link { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_NAME]", DescriptionLabel = "SELLER_NAME_VARIABLE_DESCRIPTION")]
        public string SellerName { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_EMAIL]", DescriptionLabel = "SELLER_EMAIL_VARIABLE_DESCRIPTION")]
        public string SellerEmail { get; set; }

        [MessageVariable(TemplateVariable = "[TRACKING_LINK]", DescriptionLabel = "TRACKING_LINK_VARIABLE_DESCRIPTION")]
        public string TrackingLink { get; set; }
    }
}