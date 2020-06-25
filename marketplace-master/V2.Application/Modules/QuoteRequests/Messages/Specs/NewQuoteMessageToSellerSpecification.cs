using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages.Specs
{
    public class NewQuoteMessageToSellerSpecification : MessageSpecification<QuoteMessageModel>
    {
        public override string TitleLabel => "NEW_QUOTE_SELLER_MESSAGE_TITLE";

        public override string DescriptionLabel => "NEW_QUOTE_SELLER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "NEW_QUOTE_EMAIL_SUBJECT";

        public NewQuoteMessageToSellerSpecification() : base("Quote")
        {
        }
    }
}