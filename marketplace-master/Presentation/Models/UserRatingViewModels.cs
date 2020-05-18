using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models
{
    public class CompleteRatingModel
    {
        [Required]
        public string Comment { get; set; }

        [Required]
        public int? RatingValue { get; set; }

        [Required]
        public string RatingId { get; set; }
    }

    public class CompleteRatingViewModel
    {
        public CompleteRatingModel Rating { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string TargetName { get; set; }

        public string TargetAvatar { get; set; }

        public string RatingQuoteId { get; set; }

        public string RatingOrderId { get; set; }

        public bool IsQuoteRating => !string.IsNullOrWhiteSpace(RatingQuoteId);

        public bool IsOrderRating => !string.IsNullOrWhiteSpace(RatingOrderId);
    }
}