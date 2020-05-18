using Domain.Entities;
using System;

namespace Domain.Events
{
    public class PendingRatingCreatedEvent : DomainEvent
    {
        public Person Sender { get; set; }
        public Person Target { get; set; }
        public Guid RatingId { get; set; }
        public RatingTargetRole TargetRole { get; set; }

        public enum RatingTargetRole
        {
            SELLER,
            BUYER
        }
    }
}