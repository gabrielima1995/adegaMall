using Application.Sellers;
using Domain.Entities;

namespace Presentation.Areas.Admin.Models
{
    public class UserRatingDetailsViewModel
    {
        public UserRating Rating { get; set; }



        public Person Sender { get; set; }

        public double SenderAverageRating { get; set; }

        public int SenderRatingsCount { get; set; }



        public Person Target { get; set; }

        public double TargetAverageRating { get; set; }

        public int TargetRatingsCount { get; set; }

        public UserRatingDetailsViewModel()
        {

        }

        public string GetAvatar(Person p)
        {
            return SellerHelper.GetAvatarUrl(p);
        }
    }
}