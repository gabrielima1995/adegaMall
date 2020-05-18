using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class RentalStartReminderMessageToBuyerSpecification : MessageSpecification<RentalStartReminderMessageModel>
    {
        public override string TitleLabel => "RENTAL_START_REMINDER_BUYER_MESSAGE_TITLE";

        public override string DescriptionLabel => "RENTAL_START_REMINDER_BUYER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "RENTAL_START_REMINDER_BUYER_SUBJECT";

        public RentalStartReminderMessageToBuyerSpecification() : base("Store")
        {
        }
    }
}