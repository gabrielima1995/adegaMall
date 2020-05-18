using V2.Application.Messages;

namespace V2.Application.Modules.UserAccounts.Messages.Model
{
    public class ResetPasswordMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[USER_NAME]", DescriptionLabel = "USER_NAME_VARIABLE_DESCRIPTION")]
        public string UserName { get; set; }

        [MessageVariable(TemplateVariable = "[RESET_PASSWORD_URL]", DescriptionLabel = "RESET_PASSWORD_URL_VARIABLE_DESCRIPTION")]
        public string Url { get; set; }
    }
}