using V2.Application.Messages;

namespace V2.Application.Modules.Disputes.Messages.Specs
{
    public class DisputeResolvedMessageSpecification : MessageSpecification<DisputeMessageModel>
    {
        public DisputeResolvedMessageSpecification() : base("Dispute")
        {

        }

        public override string TitleLabel => "DISPUTE_RESOLVED_MESSAGE_TITLE";

        public override string DescriptionLabel => "DISPUTE_RESOLVED_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "DISPUTE_RESOLVED_MESSAGE_TITLE";

    }
}
