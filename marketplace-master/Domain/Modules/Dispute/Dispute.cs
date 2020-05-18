using Domain.Events;
using Infra.Crosscutting.Globalization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.DisputeModule
{
    public class Dispute : BaseEntity<Guid>
    {
        private Dispute()
        {
        }

        private Dispute(Guid orderId, string buyerId, string sellerId, string authorId)
        {
            Id = Guid.NewGuid();
            DisputeComments = new List<DisputeComment>();

            OrderId = orderId.ToString();
            BuyerId = buyerId;
            SellerId = sellerId;

            Status = authorId.Equals(sellerId, StringComparison.OrdinalIgnoreCase)
                ? DisputeStatus.OpenedBySeller
                : DisputeStatus.OpenedByBuyer;
        }

        public static Dispute Start(Guid orderId, string buyerId, string sellerId, Person author)
        {
            var dispute = new Dispute(orderId, buyerId, sellerId, author.Id);
            dispute.AddDomainEvent(new DisputeOpenedEvent(dispute, author));
            return dispute;
        }

        public string OrderId { get; set; }
        public string BuyerId { get; set; }
        public string SellerId { get; set; }
        public DisputeStatus Status { get; private set; }
        public List<DisputeComment> DisputeComments { get; set; } = new List<DisputeComment>();
        public bool MediationRequested { get; private set; }

        public void SetStatus(DisputeStatus newStatus)
        {
            Status = newStatus;
            AddDomainEvent(new DisputeStatusChangedEvent(this, newStatus));

            if (newStatus.In(DisputeStatus.MediationRequestedByBuyer, DisputeStatus.MediationRequestedBySeller))
                MediationRequested = true;
        }
    }

    public class DisputeComment : BaseEntity<Guid>
    {
        [MaxLength(1000)]
        public string Message { get; set; }

        public string SenderId { get; set; }

        public bool IsSystemMessage { get; set; }

        public DisputeComment()
        {
            Id = Guid.NewGuid();
        }
    }

    public enum DisputeStatus
    {
        [DescriptionLocalized("DISPUTE_OPENED_BY_SELLER")]
        OpenedBySeller = 0,

        [DescriptionLocalized("DISPUTE_OPENED_BY_BUYER")]
        OpenedByBuyer = 1,

        [DescriptionLocalized("DISPUTE_MEDIATION_BY_SELLER")]
        MediationRequestedBySeller = 2,

        [DescriptionLocalized("DISPUTE_MEDIATION_BY_BUYER")]
        MediationRequestedByBuyer = 3,

        [DescriptionLocalized("DISPUTE_CLOSED_SELLER")]
        ClosedBySeller = 4,

        [DescriptionLocalized("DISPUTE_CLOSED_BUYER")]
        ClosedByBuyer = 5,

        [DescriptionLocalized("DISPUTE_CLOSED_ADMIN")]
        ClosedByAdmin = 6,

        [DescriptionLocalized("DISPUTE_CLOSED_IDLE")]
        ClosedForInactivity = 7
    }
}
