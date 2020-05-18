using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class RentalPeriodChangedMessageToSellerSpecification : MessageSpecification<RentalPeriodChangedMessageModel>
    {
        public override string TitleLabel => "RENTAL_PERIOD_CHANGED_SELLER_MESSAGE_TITLE";

        public override string DescriptionLabel => "RENTAL_PERIOD_CHANGED_SELLER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "RENTAL_PERIOD_CHANGED_SELLER_MESSAGE_SUBJECT";

        public RentalPeriodChangedMessageToSellerSpecification() : base("Store")
        {
        }
    }
}
