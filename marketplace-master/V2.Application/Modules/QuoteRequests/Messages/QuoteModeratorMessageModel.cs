using System;
using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages
{
    public class QuoteModeratorMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[QUOTE_ID]", DescriptionLabel = "QUOTE_ID_VARIABLE_DESCRIPTION")]
        public Guid QuoteId { get; set; }

        [MessageVariable(TemplateVariable = "[QUOTE_TITLE]", DescriptionLabel = "QUOTE_TITLE_VARIABLE_DESCRIPTION")]
        public string QuoteTitle { get; set; }

        [MessageVariable(TemplateVariable = "[TARGET_NAME]", DescriptionLabel = "TARGET_NAME_VARIABLE_DESCRIPTION")]
        public string TargetName { get; set; }

        [MessageVariable(TemplateVariable = "[PASSWORD_URL]", DescriptionLabel = "PASSWORD_URL_VARIABLE_DESCRIPTION")]
        public string SetPasswordUrl { get; set; }

        [MessageVariable(TemplateVariable = "[ANSWER]", DescriptionLabel = "ANSWER_VARIABLE_DESCRIPTION")]
        public string Answer { get; set; }

        [MessageVariable(TemplateVariable = "[QUOTE_URL]", DescriptionLabel = "QUOTE_URL_VARIABLE_DESCRIPTION")]
        public string QuoteUrl => BuildAppUrl("/quotes/details/" + QuoteId);

        [MessageVariable(TemplateVariable = "[QUOTE_URL_ADM]", DescriptionLabel = "QUOTE_URL_VARIABLE_DESCRIPTION")]
        public string QuoteUrlAdmin => BuildAppUrl("/admin/quotes/details/" + QuoteId);

        [MessageVariable(TemplateVariable = "[LOGIN_URL]", DescriptionLabel = "LOGIN_URL_VARIABLE_DESCRIPTION")]
        public string LoginUrl => BuildAppUrl("/account/login");

        [MessageVariable(TemplateVariable = "[QUOTE_CONTENT]", DescriptionLabel = "QUOTE_CONTENT_VARIABLE_DESCRIPTION")]
        public string QuoteContent { get; set; }

        public QuoteModeratorMessageModel()
        {
            SetPasswordUrl = BuildAppUrl("/account/forgotpassword");
        }
    }
}