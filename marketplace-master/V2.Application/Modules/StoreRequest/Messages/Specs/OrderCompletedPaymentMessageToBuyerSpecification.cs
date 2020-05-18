using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class OrderCompletedPaymentMessageToBuyerSpecification : MessageSpecification<OrderCompletedPaymentMessageModel>
    {
        public override string TitleLabel => "ORDER_COMPLETED_PAYMENT_BUYER_MESSAGE_TITLE";
        public override string DescriptionLabel => "ORDER_COMPLETED_PAYMENT_BUYER_MESSAGE_DESCRIPTION";
        public override string DefaultSubjectLabel => "ORDER_COMPLETED_PAYMENT_BUYER_SUBJECT";

        public OrderCompletedPaymentMessageToBuyerSpecification() : base("Store")
        {
        }
    }
}