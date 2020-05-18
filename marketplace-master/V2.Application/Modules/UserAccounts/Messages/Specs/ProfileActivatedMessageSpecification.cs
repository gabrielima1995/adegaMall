using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;

namespace V2.Application.Modules.UserAccounts.Messages.Specs
{
    public class ProfileActivatedMessageSpecification : MessageSpecification<ProfileActivatedMessageModel>
    {
        public override string TitleLabel => "PROFILE_ACTIVATED_MESSAGE_TITLE";

        public override string DescriptionLabel => "PROFILE_ACTIVATED_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "PROFILE_ACTIVATED_MESSAGE_TITLE";

        public ProfileActivatedMessageSpecification() : base("Account")
        {
        }
    }
}