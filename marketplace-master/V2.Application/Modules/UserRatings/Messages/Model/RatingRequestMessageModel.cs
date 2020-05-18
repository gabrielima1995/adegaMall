using V2.Application.Messages;

namespace V2.Application.Modules.UserRatings.Messages
{
    public class RatingRequestMessageModel : MessageModel
    {
        public string RatingId { get; set; }


        [MessageVariable(TemplateVariable = "[RATING_URL]", DescriptionLabel = "RATING_URL_DESCRIPTION")]
        public string RatingUrl => BuildAppUrl($"/userrating/sendrating/?ratingId={RatingId}");

        [MessageVariable(TemplateVariable = "[RATING_TARGET]", DescriptionLabel = "RATING_TARGET_DESCRIPTION")]
        public string RatedName { get; set; }

        [MessageVariable(TemplateVariable = "[RATING_SENDER]", DescriptionLabel = "RATING_SENDER_DESCRIPTION")]
        public string RaterName { get; set; }
    }
}
