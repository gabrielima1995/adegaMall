using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages.Specs
{
    public class NewQuoteAnswerMessageToSellerSpecification : MessageSpecification<QuoteMessageModel>
    {
        public override string TitleLabel => "NEW_QUOTE_ANSWER_SELLER_MESSAGE_TITLE";

        public override string DescriptionLabel => "NEW_QUOTE_ANSWER_SELLER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "NEW_QUOTE_ANSWER_TO_SELLER_MESSAGE_TITLE";

        public NewQuoteAnswerMessageToSellerSpecification() : base("Quote")
        {
        }
    }
}