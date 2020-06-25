using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages.Specs
{
    public class NewQuoteAcceptedMessageToSellerSpecification : MessageSpecification<QuoteMessageModel>
    {
        public override string TitleLabel => "QUOTE_ACCEPTED_BY_BUYER_MESSAGE_TITLE";

        public override string DescriptionLabel => "QUOTE_ACCEPTED_BY_BUYER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "QUOTE_ACCEPTED_BY_BUYER_SUBJECT";

        public NewQuoteAcceptedMessageToSellerSpecification() : base("Quote")
        {

        }
    }
}
