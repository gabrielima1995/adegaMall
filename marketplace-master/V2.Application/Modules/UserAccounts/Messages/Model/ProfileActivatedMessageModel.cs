using V2.Application.Messages;

namespace V2.Application.Modules.UserAccounts.Messages.Model
{
    public class ProfileActivatedMessageModel : MessageModel
    {
        [MessageVariable(TemplateVariable = "[USER_NAME]", DescriptionLabel = "USER_NAME_VARIABLE_DESCRIPTION")]
        public string UserName { get; set; }
    }
}