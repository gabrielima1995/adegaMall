using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages.Specs
{
    public class NewQuoteClosedMessageToSellerSpecification : MessageSpecification<QuoteMessageModel>
    {
        public override string TitleLabel => "QUOTE_CLOSED_BY_BUYER_MESSAGE_TITLE";

        public override string DescriptionLabel => "QUOTE_CLOSED_BY_BUYER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "QUOTE_CLOSED_BY_BUYER_SUBJECT";

        public NewQuoteClosedMessageToSellerSpecification() : base("Quote")
        {

        }
    }
}
