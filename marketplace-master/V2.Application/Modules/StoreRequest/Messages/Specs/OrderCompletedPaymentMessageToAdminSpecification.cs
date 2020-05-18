using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class OrderCompletedPaymentMessageToAdminSpecification : MessageSpecification<OrderCompletedPaymentMessageModel>
    {
        public override string TitleLabel => "ORDER_COMPLETED_PAYMENT_ADMIN_MESSAGE_TITLE";

        public override string DescriptionLabel => "ORDER_COMPLETED_PAYMENT_ADMIN_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "ORDER_COMPLETED_PAYMENT_ADMIN_SUBJECT";

        public OrderCompletedPaymentMessageToAdminSpecification() : base("Store")
        {
        }
    }
}