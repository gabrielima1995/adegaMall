using System;
using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages
{
    public class QuoteBillingCreatedMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[QUOTE_ID]", DescriptionLabel = "QUOTE_ID_VARIABLE_DESCRIPTION")]
        public Guid QuoteId { get; set; }

        [MessageVariable(TemplateVariable = "[QUOTE_TITLE]", DescriptionLabel = "QUOTE_TITLE_VARIABLE_DESCRIPTION")]
        public string QuoteTitle { get; set; }

        [MessageVariable(TemplateVariable = "[BUYER_NAME]", DescriptionLabel = "BUYER_NAME_VARIABLE_DESCRIPTION")]
        public string BuyerName { get; set; }

        [MessageVariable(TemplateVariable = "[QUOTE_URL]", DescriptionLabel = "QUOTE_URL_VARIABLE_DESCRIPTION")]
        public string QuoteUrl => BuildAppUrl("/quotes/details/" + QuoteId);

        [MessageVariable(TemplateVariable = "[BILLING_URL]", DescriptionLabel = "BILLING_URL_VARIABLE_DESCRIPTION")]
        public string BillingUrl { get; set; }

        public QuoteBillingCreatedMessageModel()
        {
        }
    }
}