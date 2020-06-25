using Domain.Entities.StoreModule;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Modules.Products.Orders
{
    public class OrderReviewViewModel
    {
        public Guid OrderId { get; set; }

        public string ReviewerId { get; set; }

        [DisplayNameLocalized("ORDER_REVIEW_RATE_LABEL")]
        [RequiredLocalized]
        [RangeLocalized(1, 100)]
        public int Rate { get; set; }

        [DisplayNameLocalized("ORDER_REVIEW_DESCRIPTION_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(2048)]
        public string Description { get; set; }

        public bool IsNew => Rate == 0;

        public OrderReviewViewModel()
        {
        }

        public OrderReviewViewModel(Guid orderId, string reviewerId)
            : this()
        {
            OrderId = orderId;
            ReviewerId = reviewerId;
        }

        public OrderReviewViewModel(OrderReview review)
            : this(review.Order.Id, review.Reviewer.Id)
        {
            Rate = review.Rate;
            Description = review.Description;
        }
    }
}