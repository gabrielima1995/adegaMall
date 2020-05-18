using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class OrderCanceledMessageToSellerSpecification : MessageSpecification<OrderCanceledMessageModel>
    {
        public override string TitleLabel => "ORDER_CANCELED_SELLER_MESSAGE_TITLE";

        public override string DescriptionLabel => "ORDER_CANCELED_SELLER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "ORDER_CANCELED_SELLER_SUBJECT";

        public OrderCanceledMessageToSellerSpecification() : base("Store")
        {
        }
    }
}