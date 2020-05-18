using V2.Application.Messages;

namespace V2.Application.Modules.StoreRequest.Messages.Specs
{
    public class RentalEndReminderMessageToSellerSpecification : MessageSpecification<RentalEndReminderMessageModel>
    {
        public override string TitleLabel => "RENTAL_END_REMINDER_SELLER_MESSAGE_TITLE";

        public override string DescriptionLabel => "RENTAL_END_REMINDER_SELLER_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "RENTAL_END_REMINDER_SELLER_SUBJECT";

        public RentalEndReminderMessageToSellerSpecification() : base("Store") 
        { 
        }
    }
}
