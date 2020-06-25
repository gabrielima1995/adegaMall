using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages.Specs
{
    public class NewQuoteAnswerMessageToBuyerSpecification : MessageSpecification<QuoteMessageModel>
    {
        public override string TitleLabel => "NEW_QUOTE_ANSWER_BUYER_MESSAGE_TITLE";

        public override string DescriptionLabel => "NEW_QUOTE_ANSWER_BUYER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "NEW_QUOTE_ANSWER_TO_BUYER_MESSAGE_TITLE";

        public NewQuoteAnswerMessageToBuyerSpecification() : base("Quote")
        {
        }
    }
}