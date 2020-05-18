using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class MarketplacePushSubscription : BaseEntity<Guid>
    {
        public MarketplacePushSubscription()
        {
            Id = Guid.NewGuid();
        }

        [Index(IsUnique = false)]
        public string Token { get; set; }

        public string UserId { get; set; }

        public string UserRole { get; set; }
    }
}