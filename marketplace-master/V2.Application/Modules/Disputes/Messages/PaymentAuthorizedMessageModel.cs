using System;
using V2.Application.Messages;

namespace V2.Application.Modules.Disputes.Messages
{
    public class PaymentAuthorizedMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[ORDER_ID]", DescriptionLabel = "ORDER_ID_VARIABLE_DESCRIPTION")]
        public Guid OrderId { get; set; }

        [MessageVariable(TemplateVariable = "[RECIPIENT_NAME]", DescriptionLabel = "RECIPIENT_NAME_VARIABLE_DESCRIPTION")]
        public string RecipientName { get; set; }

        [MessageVariable(TemplateVariable = "[BUYER_NAME]", DescriptionLabel = "BUYER_NAME_VARIABLE_DESCRIPTION")]
        public string BuyerName { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_NAME]", DescriptionLabel = "SELLER_NAME_VARIABLE_DESCRIPTION")]
        public string SellerName { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_URL]", DescriptionLabel = "ORDER_URL_VARIABLE_DESCRIPTION")]
        public string OrderUrl => BuildAppUrl($"/mysales/details/{OrderId}");


        public PaymentAuthorizedMessageModel()
        {

        }
    }
}