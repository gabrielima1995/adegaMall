using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;

namespace V2.Application.Modules.UserAccounts.Messages.Specs
{
    public class BuyerProfileActivatedMessageSpecification : MessageSpecification<ProfileActivatedMessageModel>
    {
        public override string TitleLabel => "BUYER_PROFILE_ACTIVATED_MESSAGE_TITLE";

        public override string DescriptionLabel => "BUYER_PROFILE_ACTIVATED_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "BUYER_PROFILE_ACTIVATED_MESSAGE_TITLE";

        public BuyerProfileActivatedMessageSpecification() : base("Account")
        {
        }
    }
}