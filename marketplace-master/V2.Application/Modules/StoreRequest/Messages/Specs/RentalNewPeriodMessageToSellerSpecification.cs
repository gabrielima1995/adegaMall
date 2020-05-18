using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class RentalNewPeriodMessageToSellerSpecification : MessageSpecification<RentalPeriodChangedMessageModel>
    {
        public override string TitleLabel => "RENTAL_NEW_PERIOD_SELLER_MESSAGE_TITLE";

        public override string DescriptionLabel => "RENTAL_NEW_PERIOD_SELLER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "RENTAL_NEW_PERIOD_SELLER_MESSAGE_SUBJECT";

        public RentalNewPeriodMessageToSellerSpecification() : base("Store")
        {
        }
    }
}
