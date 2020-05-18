using V2.Application.Messages;

namespace V2.Application.Modules.Subscriptions.Messages.Specs
{
    public class NewSubscriptionMessageToSellerSpecification : MessageSpecification<NewSubscriptionMessageModel>
    {
        public override string TitleLabel => "NEW_SUBSCRIPTION_SELLER_MESSAGE_TITLE";

        public override string DescriptionLabel => "NEW_SUBSCRIPTION_SELLER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "NEW_SUBSCRIPTION_SUBJECT";

        public NewSubscriptionMessageToSellerSpecification() : base("Subscription")
        {
        }
    }
}