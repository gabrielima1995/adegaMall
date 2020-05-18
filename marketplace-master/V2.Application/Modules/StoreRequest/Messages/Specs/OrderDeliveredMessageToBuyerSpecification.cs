using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class OrderDeliveredMessageToBuyerSpecification : MessageSpecification<OrderDeliveredMessageModel>
    {
        public override string TitleLabel => "ORDER_DELIVERED_BUYER_MESSAGE_TITLE";

        public override string DescriptionLabel => "ORDER_DELIVERED_BUYER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "ORDER_DELIVERED_BUYER_SUBJECT";

        public OrderDeliveredMessageToBuyerSpecification() : base("Store")
        {
        }
    }
}