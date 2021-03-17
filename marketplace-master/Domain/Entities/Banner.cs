using System;

namespace Domain.Entities
{
    public class Banner : SoftDeletableEntity<Guid>
    {
        public string ImageUrl { get; set; }
        
        public string MobileImageUrl { get; set; }

        public string ActionUrl { get; set; }

        public DateTime? StartsAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public int Order { get; set; }

        public Banner()
        {
            Id = Guid.NewGuid();
        }
    }
}
