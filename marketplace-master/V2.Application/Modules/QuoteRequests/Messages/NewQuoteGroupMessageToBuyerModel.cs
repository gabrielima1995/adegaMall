using System;
using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages
{
    public class NewQuoteGroupMessageToBuyerModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[QUOTE_GROUP_ID]", DescriptionLabel = "QUOTE_GROUP_ID_VARIABLE_DESCRIPTION")]
        public Guid GroupId { get; set; }

        [MessageVariable(TemplateVariable = "[QUOTE_TITLE]", DescriptionLabel = "QUOTE_TITLE_VARIABLE_DESCRIPTION")]
        public string QuoteTitle { get; set; }

        [MessageVariable(TemplateVariable = "[BUYER_NAME]", DescriptionLabel = "BUYER_NAME_VARIABLE_DESCRIPTION")]
        public string BuyerName { get; set; }

        [MessageVariable(TemplateVariable = "[PASSWORD_URL]", DescriptionLabel = "PASSWORD_URL_VARIABLE_DESCRIPTION")]
        public string SetPasswordUrl { get; set; }

        [MessageVariable(TemplateVariable = "[QUOTE_GROUP_URL]", DescriptionLabel = "QUOTE_GROUP_URL_VARIABLE_DESCRIPTION")]
        public string QuoteUrl => BuildAppUrl("/quotes/groupdetails/" + GroupId);

        [MessageVariable(TemplateVariable = "[LOGIN_URL]", DescriptionLabel = "LOGIN_URL_VARIABLE_DESCRIPTION")]
        public string LoginUrl => BuildAppUrl("/account/login");

        public NewQuoteGroupMessageToBuyerModel()
        {
            SetPasswordUrl = BuildAppUrl("/account/forgotpassword");
        }
    }
}
