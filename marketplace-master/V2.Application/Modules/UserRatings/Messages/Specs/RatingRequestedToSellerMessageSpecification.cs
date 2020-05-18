using V2.Application.Messages;

namespace V2.Application.Modules.UserRatings.Messages.Specs
{
    public class RatingRequestedToSellerMessageSpecification : MessageSpecification<RatingRequestMessageModel>
    {
        public RatingRequestedToSellerMessageSpecification() : base("Ratings")
        {
        }

        public override string TitleLabel => "RATING_REQUEST_SELLER_MESSAGE_TITLE";
        public override string DescriptionLabel => "RATING_REQUEST_SELLER_MESSAGE_DESC";
        public override string DefaultSubjectLabel => "RATING_REQUEST_SELLER_MESSAGE_SUBJECT";
    }
}
