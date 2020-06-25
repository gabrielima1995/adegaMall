using System;
using System.Collections.Generic;

namespace Application.Modules.Ratings.Models
{
    public class UserRatingsOverviewModel
    {
        public string SellerName { get; set; }

        public string SellerAvatarUrl { get; set; }

        public double SellerAverageRating { get; set; }

        public int TotalRatingsCount { get; set; }

        public List<UserRatingViewModel> LatestRatingsList { get; set; } = new List<UserRatingViewModel>();
    }

    public class UserRatingViewModel
    {
        public string SenderAvatar { get; set; }

        public string SenderName { get; set; }

        public string RatingMessage { get; set; }

        public DateTime CreatedAt { get; set; }

        public int RatingValue { get; set; }
    }
}