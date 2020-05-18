using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class OrderAwaitingDeliveryMessageToSellerSpecification : MessageSpecification<OrderAwaitingDeliveryOrPickupMessageModel>
    {
        public OrderAwaitingDeliveryMessageToSellerSpecification() : base("Store")
        {
        }

        public override string TitleLabel => "ORDER_AWAITING_DELIVERY_SELLER_MESSAGE_TITLE";
        public override string DescriptionLabel => "ORDER_AWAITING_DELIVERY_SELLER_MESSAGE_DESCRIPTION";
        public override string DefaultSubjectLabel => "ORDER_AWAITING_DELIVERY_SELLER_SUBJECT";
    }
}