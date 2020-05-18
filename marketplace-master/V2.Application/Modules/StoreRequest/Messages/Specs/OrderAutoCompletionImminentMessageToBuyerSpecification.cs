using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class OrderAutoCompletionImminentMessageToBuyerSpecification : MessageSpecification<OrderAutoCompletionImminentMessageModel>
    {
        public override string TitleLabel => "ORDER_AUTOCOMPLETE_WARNING_MESSAGE_TITLE";

        public override string DescriptionLabel => "ORDER_AUTOCOMPLETE_WARNING_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "ORDER_AUTOCOMPLETE_WARNING_MESSAGE_TITLE";

        public OrderAutoCompletionImminentMessageToBuyerSpecification() : base("Store")
        {

        }
    }
}