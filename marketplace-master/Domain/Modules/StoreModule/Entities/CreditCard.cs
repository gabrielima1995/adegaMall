using PaymentIntegrator.Core.PaymentMethods;
using System;

namespace Domain.Entities.StoreModule
{
    public class CreditCard : BaseEntity<Guid>, ICreditCardResult
    {
        public string HolderName { get; set; }

        public string DisplayNumber { get; set; }

        public string Brand { get; set; }

        public string Token { get; set; }

        public string ReferenceId { get; set; }
    }
}