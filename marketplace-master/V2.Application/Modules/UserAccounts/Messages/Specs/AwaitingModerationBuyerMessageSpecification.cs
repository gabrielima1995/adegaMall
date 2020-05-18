using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;

namespace V2.Application.Modules.UserAccounts.Messages.Specs
{
    public class AwaitingModerationBuyerMessageSpecification : MessageSpecification<AwaitingModerationBuyerMessageModel>
    {
        public override string TitleLabel => "AWAITING_MODERATION_BUYER_TITLE";

        public override string DescriptionLabel => "AWAITING_MODERATION_BUYER_DESCRIPTION";

        public override string DefaultSubjectLabel => "AWAITING_MODERATION_BUYER_SUBJECT";

        public AwaitingModerationBuyerMessageSpecification() : base("Account")
        {

        }
    }
}