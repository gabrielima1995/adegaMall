using V2.Application.Messages;
using V2.Application.Modules.UserAccounts.Messages.Model;

namespace V2.Application.Modules.UserAccounts.Messages.Specs
{
    public class AwaitingModerationMessageSpecification : MessageSpecification<AwaitingModerationMessageModel>
    {
        public override string TitleLabel => "AWAITING_MODERATION_TITLE";

        public override string DescriptionLabel => "AWAITING_MODERATION_DESCRIPTION";

        public override string DefaultSubjectLabel => "AWAITING_MODERATION_SUBJECT";

        public AwaitingModerationMessageSpecification() : base("Account")
        {

        }
    }
}