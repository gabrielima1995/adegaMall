using V2.Application.Messages;

namespace V2.Application.Modules.Disputes.Messages.Specs
{
    public class PaymentAuthorizedMessageToSellerSpecification : MessageSpecification<PaymentAuthorizedMessageModel>
    {
        public PaymentAuthorizedMessageToSellerSpecification() : base("Dispute")
        {

        }

        public override string TitleLabel => "PAYMENT_AUTHORIZED_MESSAGE_TITLE";

        public override string DescriptionLabel => "PAYMENT_AUTHORIZED_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "PAYMENT_AUTHORIZED_MESSAGE_TITLE";

    }
}
