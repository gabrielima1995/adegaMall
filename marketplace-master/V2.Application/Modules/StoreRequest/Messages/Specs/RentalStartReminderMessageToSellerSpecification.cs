using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class RentalStartReminderMessageToSellerSpecification : MessageSpecification<RentalStartReminderMessageModel>
    {
        public override string TitleLabel => "RENTAL_START_REMINDER_SELLER_MESSAGE_TITLE";

        public override string DescriptionLabel => "RENTAL_START_REMINDER_SELLER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "RENTAL_START_REMINDER_SELLER_SUBJECT";

        public RentalStartReminderMessageToSellerSpecification() : base("Store")
        {
        }
    }
}
