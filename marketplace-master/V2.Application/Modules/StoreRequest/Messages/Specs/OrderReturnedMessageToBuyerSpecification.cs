using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class OrderReturnedMessageToBuyerSpecification : MessageSpecification<OrderReturnedMessageModel>
    {
        public override string TitleLabel => "ORDER_RETURNED_BUYER_MESSAGE_TITLE";

        public override string DescriptionLabel => "ORDER_RETURNED_BUYER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "ORDER_RETURNED_BUYER_SUBJECT";

        public OrderReturnedMessageToBuyerSpecification() : base("Store")
        {
        }
    }
}