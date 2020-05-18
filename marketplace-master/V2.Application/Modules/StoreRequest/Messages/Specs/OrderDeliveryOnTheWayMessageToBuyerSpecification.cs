using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class OrderDeliveryOnTheWayMessageToBuyerSpecification : MessageSpecification<OrderDeliveryOnTheWayMessageModel>
    {
        public override string TitleLabel => "ORDER_DELIVERY_ON_THE_WAY_BUYER_MESSAGE_TITLE";

        public override string DescriptionLabel => "ORDER_DELIVERY_ON_THE_WAY_BUYER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "ORDER_DELIVERY_ON_THE_WAY_BUYER_SUBJECT";

        public OrderDeliveryOnTheWayMessageToBuyerSpecification() : base("Store")
        {
        }
    }
}