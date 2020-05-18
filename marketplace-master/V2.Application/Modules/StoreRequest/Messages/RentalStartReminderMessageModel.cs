using System.Collections.Generic;
using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages
{
    public class RentalStartReminderMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[BUYER_NAME]", DescriptionLabel = "BUYER_NAME_VARIABLE_DESCRIPTION")]
        public string BuyerName { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_ID]", DescriptionLabel = "ORDER_ID_VARIABLE_DESCRIPTION")]
        public string OrderId { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_STATUS]", DescriptionLabel = "ORDER_STATUS_VARIABLE_DESCRIPTION")]
        public string OrderStatus { get; set; }

        [MessageVariable(TemplateVariable = "[SHIPPING_METHOD]", DescriptionLabel = "SHIPPING_METHOD_VARIABLE_DESCRIPTION")]
        public string ShippingMethod { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_PRODUCTS]", DescriptionLabel = "ORDER_PRODUCTS_VARIABLE_DESCRIPTION",
            RazorTemplateResource = "OrderProducts")]
        public IEnumerable<OrderProductMessageModel> Products { get; set; }

        [MessageVariable(TemplateVariable = "[ORDER_LINK]", DescriptionLabel = "ORDER_LINK_VARIABLE_DESCRIPTION")]
        public string Link { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_NAME]", DescriptionLabel = "SELLER_NAME_VARIABLE_DESCRIPTION")]
        public string SellerName { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_EMAIL]", DescriptionLabel = "SELLER_EMAIL_VARIABLE_DESCRIPTION")]
        public string SellerEmail { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_PHONE]", DescriptionLabel = "SELLER_PHONE_VARIABLE_DESCRIPTION")]
        public string SellerPhone { get; set; }

        [MessageVariable(TemplateVariable = "[RENTAL_START_DATE]", DescriptionLabel = "RENTAL_START_DATE_VARIABLE_DESCRIPTION")]
        public string RentalStartDate { get; set; }

        [MessageVariable(TemplateVariable = "[RENTAL_END_DATE]", DescriptionLabel = "RENTAL_END_DATE_VARIABLE_DESCRIPTION")]
        public string RentalEndDate { get; set; }
    }
}