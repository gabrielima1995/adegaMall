using V2.Application.Messages;

namespace V2.Application.Modules.QuoteRequests.Messages.Specs
{
    public class NewQuoteMessageToAdminSpecification : MessageSpecification<QuoteMessageModel>
    {
        public override string TitleLabel => "NEW_QUOTE_ADMIN_MESSAGE_TITLE";

        public override string DescriptionLabel => "NEW_QUOTE_ADMIN_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "NEW_QUOTE_ADMIN_SUBJECT";

        public NewQuoteMessageToAdminSpecification() : base("Quote")
        {
        }
    }
}