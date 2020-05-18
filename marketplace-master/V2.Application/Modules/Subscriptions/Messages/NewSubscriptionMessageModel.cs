using V2.Application.Messages;

namespace V2.Application.Modules.Subscriptions.Messages
{
    public class NewSubscriptionMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[SELLER_NAME]", DescriptionLabel = "SELLER_NAME_VARIABLE_DESCRIPTION")]
        public string SellerName { get; set; }

        [MessageVariable(TemplateVariable = "[PROFILE_URL]", DescriptionLabel = "PROFILE_URL_VARIABLE_DESCRIPTION")]
        public string ProfileUrl { get; set; }
    }
}