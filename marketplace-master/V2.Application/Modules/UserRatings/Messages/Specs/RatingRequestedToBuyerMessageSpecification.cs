using V2.Application.Messages;

namespace V2.Application.Modules.UserRatings.Messages.Specs
{
    public class RatingRequestedToBuyerMessageSpecification : MessageSpecification<RatingRequestMessageModel>
    {
        public RatingRequestedToBuyerMessageSpecification() : base("Ratings")
        {
        }

        public override string TitleLabel => "RATING_REQUEST_BUYER_MESSAGE_TITLE";
        public override string DescriptionLabel => "RATING_REQUEST_BUYER_MESSAGE_DESC";
        public override string DefaultSubjectLabel => "RATING_REQUEST_BUYER_MESSAGE_SUBJECT";
    }
}
