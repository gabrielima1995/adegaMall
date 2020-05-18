using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class OrderProcessingPaymentMessageToSellerSpecification : MessageSpecification<OrderProcessingPaymentMessageModel>
    {
        public override string TitleLabel => "ORDER_PROCESSING_PAYMENT_SELLER_MESSAGE_TITLE";

        public override string DescriptionLabel => "ORDER_PROCESSING_PAYMENT_SELLER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "ORDER_PROCESSING_PAYMENT_SELLER_SUBJECT";

        public OrderProcessingPaymentMessageToSellerSpecification() : base("Store")
        {
        }
    }
}