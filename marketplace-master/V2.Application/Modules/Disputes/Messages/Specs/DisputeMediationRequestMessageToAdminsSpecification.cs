using V2.Application.Messages;

namespace V2.Application.Modules.Disputes.Messages.Specs
{
    public class DisputeMediationRequestMessageToAdminsSpecification : MessageSpecification<DisputeMessageModel>
    {
        public DisputeMediationRequestMessageToAdminsSpecification() : base("Dispute")
        {

        }

        public override string TitleLabel => "MEDIATION_REQUESTED_MESSAGE_TITLE";

        public override string DescriptionLabel => "MEDIATION_REQUESTED_MESSAGE_DESCRIPTION";

        public override string DefaultSubjectLabel => "MEDIATION_REQUESTED_MESSAGE_TITLE";

    }
}
