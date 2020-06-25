using V2.Application.Messages;

namespace V2.Application.Modules.SellerAssistantModule
{
    public class SellerAssistantRegisteredMessageSpecification : MessageSpecification<SellerAssistantRegisteredMessageModel>
    {
        public override string TitleLabel => "SELLER_ASSISTANT_REGISTERED_MESSAGE_TITLE";

        public override string DescriptionLabel => "SELLER_ASSISTANT_REGISTERED_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "SELLER_ASSISTANT_REGISTERED_MESSAGE_SUBJECT";

        public SellerAssistantRegisteredMessageSpecification() : base("SellerAssistant")
        {
        }
    }

    public class SellerAssistantRegisteredMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[ASSISTANT_NAME]", DescriptionLabel = "ASSISTANT_NAME_VARIABLE_DESCRIPTION")]
        public string Name { get; set; }

        [MessageVariable(TemplateVariable = "[ASSISTANT_EMAIL]", DescriptionLabel = "ASSISTANT_EMAIL_VARIABLE_DESCRIPTION")]
        public string Email { get; set; }

        [MessageVariable(TemplateVariable = "[ASSISTANT_PASSWORD]", DescriptionLabel = "ASSISTANT_PASSWORD_VARIABLE_DESCRIPTION")]
        public string Password { get; set; }

        [MessageVariable(TemplateVariable = "[LOGIN_URL]", DescriptionLabel = "ASSISTANT_LOGIN_URL_VARIABLE_DESCRIPTION")]
        public string LoginUrl => BuildAppUrl("/account/login");

        [MessageVariable(TemplateVariable = "[SELLER_NAME]", DescriptionLabel = "ASSISTANT_SELLER_NAME_VARIABLE_DESCRIPTION")]
        public string SellerName { get; set; }

        [MessageVariable(TemplateVariable = "[SELLER_EMAIL]", DescriptionLabel = "ASSISTANT_SELLER_EMAIL_VARIABLE_DESCRIPTION")]
        public string SellerEmail { get; set; }
    }
}
