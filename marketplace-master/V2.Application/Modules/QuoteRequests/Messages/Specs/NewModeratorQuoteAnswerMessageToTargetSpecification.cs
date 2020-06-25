using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages.Specs
{
    public class NewModeratorQuoteAnswerMessageToTargetSpecification : MessageSpecification<QuoteModeratorMessageModel>
    {
        public override string TitleLabel => "NEW_MODERATOR_QUOTE_ANSWER_TARGET_MESSAGE_TITLE";

        public override string DescriptionLabel => "NEW_MODERATOR_QUOTE_ANSWER_TARGET_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "NEW_MODERATOR_QUOTE_ANSWER_TO_TARGET_MESSAGE_TITLE";

        public NewModeratorQuoteAnswerMessageToTargetSpecification() : base("Quote")
        {
        }
    }
}