using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages.Specs
{
    public class NewQuoteBillingMessageToBuyerSpecification : MessageSpecification<QuoteBillingCreatedMessageModel>
    {
        public override string TitleLabel => "NEW_QUOTE_BILLING_BUYER_MESSAGE_TITLE";
        public override string DescriptionLabel => "NEW_QUOTE_BILLING_BUYER_MESSAGE_DESCRIPTION";
        public override string DefaultSubjectLabel => "NEW_QUOTE_BILLING_EMAIL_BUYER_SUBJECT";

        public NewQuoteBillingMessageToBuyerSpecification() : base("Quote")
        {
        }
    }
}