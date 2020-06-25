using System;
using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages
{
    public class QuoteAnswerFilteredMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[QUOTE_ID]", DescriptionLabel = "QUOTE_ID_VARIABLE_DESCRIPTION")]
        public Guid QuoteId { get; set; }

        [MessageVariable(TemplateVariable = "[QUOTE_TITLE]", DescriptionLabel = "QUOTE_TITLE_VARIABLE_DESCRIPTION")]
        public string QuoteTitle { get; set; }

        [MessageVariable(TemplateVariable = "[BUYER_NAME]", DescriptionLabel = "BUYER_NAME_VARIABLE_DESCRIPTION")]
        public string BuyerName { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_NAME]", DescriptionLabel = "SELLER_NAME_VARIABLE_DESCRIPTION")]
        public string SellerName { get; set; }

        [MessageVariable(TemplateVariable = "[ANSWER]", DescriptionLabel = "ANSWER_VARIABLE_DESCRIPTION")]
        public string Answer { get; set; }

        [MessageVariable(TemplateVariable = "[QUOTE_URL_ADM]", DescriptionLabel = "QUOTE_URL_VARIABLE_DESCRIPTION")]
        public string QuoteUrlAdmin => BuildAppUrl("/admin/quotes/details/" + QuoteId);

        public QuoteAnswerFilteredMessageModel()
        {
        }
    }
}