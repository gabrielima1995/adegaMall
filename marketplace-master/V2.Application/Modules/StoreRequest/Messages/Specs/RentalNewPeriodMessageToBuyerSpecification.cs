using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class RentalNewPeriodMessageToBuyerSpecification : MessageSpecification<RentalPeriodChangedMessageModel>
    {
        public override string TitleLabel => "RENTAL_NEW_PERIOD_BUYER_MESSAGE_TITLE";

        public override string DescriptionLabel => "RENTAL_NEW_PERIOD_BUYER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "RENTAL_NEW_PERIOD_BUYER_MESSAGE_SUBJECT";

        public RentalNewPeriodMessageToBuyerSpecification() : base("Store")
        {
        }
    }
}
