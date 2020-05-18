using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages
{
    public class RentalPeriodChangedMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[BUYER_NAME]", DescriptionLabel = "BUYER_NAME_VARIABLE_DESCRIPTION")]
        public string BuyerName { get; set; }

        [MessageVariable(TemplateVariable = "[BUYER_EMAIL]", DescriptionLabel = "BUYER_EMAIL_VARIABLE_DESCRIPTION")]
        public string BuyerEmail { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_NAME]", DescriptionLabel = "SELLER_NAME_VARIABLE_DESCRIPTION")]
        public string SellerName { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_EMAIL]", DescriptionLabel = "SELLER_EMAIL_VARIABLE_DESCRIPTION")]
        public string SellerEmail { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_ID]", DescriptionLabel = "ORDER_ID_VARIABLE_DESCRIPTION")]
        public string OrderId { get; set; }

        [MessageVariable(TemplateVariable = "[PRODUCT_NAME]", DescriptionLabel = "PRODUCT_NAME_VARIABLE_DESCRIPTION")]
        public string ProductName { get; set; }

        [MessageVariable(TemplateVariable = "[RENTAL_START_DATE]", DescriptionLabel = "RENTAL_START_DATE_VARIABLE_DESCRIPTION")]
        public string RentalStartDate { get; set; }

        [MessageVariable(TemplateVariable = "[RENTAL_END_DATE]", DescriptionLabel = "RENTAL_END_DATE_VARIABLE_DESCRIPTION")]
        public string RentalEndDate { get; set; }

        [MessageVariable(TemplateVariable = "[EXTRA_INFO]", DescriptionLabel = "EXTRA_INFO_VARIABLE_DESCRIPTION")]
        public string ExtraInfo { get; set; }

        [MessageConditionalVariable("[IF:HAS_EXTRA_INFO]", "HAS_EXTRA_INFO_VARIABLE_DESCRIPTION", "Model.HasExtraInfo")]
        public bool HasExtraInfo => !string.IsNullOrWhiteSpace(ExtraInfo);
    }
}