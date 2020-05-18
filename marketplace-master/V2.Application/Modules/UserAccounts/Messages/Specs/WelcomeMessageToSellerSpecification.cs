using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;

namespace V2.Application.Modules.UserAccounts.Messages.Specs
{
    public class WelcomeMessageToSellerSpecification : MessageSpecification<WelcomeMessageModel>
    {
        public WelcomeMessageToSellerSpecification() : base("Account")
        {
        }

        public override string TitleLabel => "WELCOME_MESSAGE_TO_SELLER_TITLE";
        public override string DescriptionLabel => "WELCOME_MESSAGE_TO_SELLER_DESCRIPTION";
        public override string DefaultSubjectLabel => "WELCOME_MESSAGE_TO_SELLER_TITLE";
    }
}