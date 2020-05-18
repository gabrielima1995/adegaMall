using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;

namespace V2.Application.Modules.UserAccounts.Messages.Specs
{
    public class WelcomeMessageSpecification : MessageSpecification<WelcomeMessageModel>
    {
        public WelcomeMessageSpecification() : base("Account")
        {
        }

        public override string TitleLabel => "WELCOME_MESSAGE_TITLE";
        public override string DescriptionLabel => "WELCOME_MESSAGE_DESCRIPTION_V2";
        public override string DefaultSubjectLabel => "WELCOME_MESSAGE_TITLE";
    }
}