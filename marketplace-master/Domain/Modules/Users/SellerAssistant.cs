using System;

namespace Domain.Entities
{
    public class SellerAssistant : Person
    {
        private SellerAssistant() { }
        public SellerAssistant(string userId, string email, string sellerId) : base(userId, email, false)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            if (string.IsNullOrEmpty(sellerId)) throw new ArgumentNullException(nameof(sellerId));

            FullName = email;
            SellerId = sellerId;
        }

        public string SellerId { get; private set; }
        public override PersonType PersonType => PersonType.SellerAssistant;
    }
}