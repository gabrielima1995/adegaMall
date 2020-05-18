using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class RentalPeriodChangedMessageToBuyerSpecification : MessageSpecification<RentalPeriodChangedMessageModel>
    {
        public override string TitleLabel => "RENTAL_PERIOD_CHANGED_BUYER_MESSAGE_TITLE";

        public override string DescriptionLabel => "RENTAL_PERIOD_CHANGED_BUYER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "RENTAL_PERIOD_CHANGED_BUYER_MESSAGE_SUBJECT";

        public RentalPeriodChangedMessageToBuyerSpecification() : base("Store")
        {
        }
    }
}
