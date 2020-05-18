using System;
using System.Collections.Generic;
using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages
{
    public class OrderProcessingPaymentMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[BUYER_NAME]", DescriptionLabel = "BUYER_NAME_VARIABLE_DESCRIPTION")]
        public string BuyerName { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_ID]", DescriptionLabel = "ORDER_ID_VARIABLE_DESCRIPTION")]
        public string OrderId { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_STATUS]", DescriptionLabel = "ORDER_STATUS_VARIABLE_DESCRIPTION")]
        public string OrderStatus { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_NAME]", DescriptionLabel = "SELLER_NAME_VARIABLE_DESCRIPTION")]
        public string SellerName { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_AMOUNT]", DescriptionLabel = "ORDER_AMOUNT_VARIABLE_DESCRIPTION")]
        public string Amount { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_PRODUCTS]", DescriptionLabel = "ORDER_PRODUCTS_VARIABLE_DESCRIPTION", RazorTemplateResource = "OrderProducts")]
        public IEnumerable<OrderProductMessageModel> Products { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_LINK]", DescriptionLabel = "ORDER_LINK_VARIABLE_DESCRIPTION")]
        public string Link { get; set; }

        [MessageVariable(TemplateVariable = "[PRODUCT_LINKS]", DescriptionLabel = "PRODUCT_LINKS_VARIABLE_DESCRIPTION", RazorTemplateResource = "ProductLinks")]
        public IEnumerable<string> Links { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_EMAIL]", DescriptionLabel = "SELLER_EMAIL_VARIABLE_DESCRIPTION")]
        public string SellerEmail { get; set; }

        [MessageVariable(TemplateVariable = "[BUYER_PHONE]", DescriptionLabel = "BUYER_PHONE_VARIABLE_DESCRIPTION")]
        public string BuyerPhone { get; set; }

        [MessageConditionalVariable("[IF:IS_OFFLINE_PAYMENT]", "ORDER_IS_OFFLINE_PAYMENT_VARIABLE_DESCRIPTION", "Model.IsOfflinePayment")]
        public bool IsOfflinePayment { get; set; }

        [Obsolete("This variable uses a sub-template, which the user cannot edit. Is is kept here while there're marketplaces still using the old template, which included this variable.")]
        [MessageVariable(TemplateVariable = "[ORDER_OFFLINE_INSTRUCTIONS]", DescriptionLabel = "ORDER_OFFLINE_INSTRUCTIONS_VARIABLE_DESCRIPTION", RazorTemplateResource = "OrderOfflineInstructions", HideFromUI = true)]
        public bool IsOfflinePayment_Old => IsOfflinePayment;

        [MessageVariable(TemplateVariable = "[EXTRA_INFO]", DescriptionLabel = "EXTRA_INFO_VARIABLE_DESCRIPTION")]
        public string ExtraInfo { get; set; }

        [MessageConditionalVariable("[IF:HAS_EXTRA_INFO]", "HAS_EXTRA_INFO_VARIABLE_DESCRIPTION", "Model.HasExtraInfo")]
        public bool HasExtraInfo => !string.IsNullOrWhiteSpace(ExtraInfo);

        [MessageVariable(TemplateVariable = "[RENTAL_START_DATE]", DescriptionLabel = "RENTAL_START_DATE_VARIABLE_DESCRIPTION")]
        public string RentalStartDate { get; set; }

        [MessageVariable(TemplateVariable = "[RENTAL_END_DATE]", DescriptionLabel = "RENTAL_END_DATE_VARIABLE_DESCRIPTION")]
        public string RentalEndDate { get; set; }
    }
}