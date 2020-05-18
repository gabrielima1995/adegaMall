using System.Collections.Generic;
using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages
{
    public class OrderCanceledMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[BUYER_NAME]", DescriptionLabel = "BUYER_NAME_VARIABLE_DESCRIPTION")]
        public string BuyerName { get; set; }

        [MessageVariable(TemplateVariable = "[BUYER_EMAIL]", DescriptionLabel = "BUYER_EMAIL_VARIABLE_DESCRIPTION")]
        public string BuyerEmail { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_ID]", DescriptionLabel = "ORDER_ID_VARIABLE_DESCRIPTION")]
        public string OrderId { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_NAME]", DescriptionLabel = "SELLER_NAME_VARIABLE_DESCRIPTION")]
        public string SellerName { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_AMOUNT]", DescriptionLabel = "ORDER_AMOUNT_VARIABLE_DESCRIPTION")]
        public string Amount { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_PRODUCTS]", DescriptionLabel = "ORDER_PRODUCTS_VARIABLE_DESCRIPTION",
            RazorTemplateResource = "OrderProducts")]
        public IEnumerable<OrderProductMessageModel> Products { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_CANCEL_EXPLANATION]", DescriptionLabel = "ORDER_CANCEL_EXPLANATION_VARIABLE_DESCRIPTION")]
        public string CancelExplanation { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_EMAIL]", DescriptionLabel = "SELLER_EMAIL_VARIABLE_DESCRIPTION")]
        public string SellerEmail { get; set; }

        [MessageVariable(TemplateVariable = "[EXTRA_INFO]", DescriptionLabel = "EXTRA_INFO_VARIABLE_DESCRIPTION")]
        public string ExtraInfo { get; set; }

        [MessageConditionalVariable("[IF:HAS_EXTRA_INFO]", "HAS_EXTRA_INFO_VARIABLE_DESCRIPTION", "Model.HasExtraInfo")]
        public bool HasExtraInfo => !string.IsNullOrWhiteSpace(ExtraInfo);
    }
}