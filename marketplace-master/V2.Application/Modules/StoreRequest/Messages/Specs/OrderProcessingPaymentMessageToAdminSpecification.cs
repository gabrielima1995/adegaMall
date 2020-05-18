using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class OrderProcessingPaymentMessageToAdminSpecification : MessageSpecification<OrderProcessingPaymentMessageModel>
    {
        public override string TitleLabel => "ORDER_PROCESSING_PAYMENT_ADMIN_MESSAGE_TITLE";

        public override string DescriptionLabel => "ORDER_PROCESSING_PAYMENT_ADMIN_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "ORDER_PROCESSING_PAYMENT_ADMIN_SUBJECT";

        public OrderProcessingPaymentMessageToAdminSpecification() : base("Store")
        {
        }
    }
}