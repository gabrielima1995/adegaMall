using V2.Application.Messages;

namespace V2.Application.Modules.Disputes.Messages.Specs
{
    public class DisputeOpenedMessageSpecification : MessageSpecification<DisputeMessageModel>
    {
        public DisputeOpenedMessageSpecification() : base("Dispute")
        {

        }

        public override string TitleLabel => "DISPUTE_OPENED_MESSAGE_TITLE";

        public override string DescriptionLabel => "DISPUTE_OPENED_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "DISPUTE_OPENED_MESSAGE_TITLE";

    }
}
