using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages.Specs
{
    public class QuoteAnswerFilteredMessageToAdminSpecification : MessageSpecification<QuoteAnswerFilteredMessageModel>
    {
        public override string TitleLabel => "QUOTE_ANSWER_FILTERED_MESSAGE_TITLE";

        public override string DescriptionLabel => "QUOTE_ANSWER_FILTERED_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "QUOTE_ANSWER_FILTERED_MESSAGE_SUBJECT";

        public QuoteAnswerFilteredMessageToAdminSpecification() : base("Quote")
        {
        }
    }
}