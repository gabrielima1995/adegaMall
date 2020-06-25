using PaymentIntegrator.Core.Subscriptions;
using System;

namespace Application.Modules.Subscriptions.Moip
{
    public class MoipPostback : SubscriptionPostbackBase
    {
        public override SubscriptionStatus Status { get; }

        public override decimal TransactionAmount { get; }

        public override DateTime SubscriptionDate => DateTime.UtcNow;

        public override string GatewayId { get; }

        public override string Currency => "BRL";

        public override string PostbackUrl => "";

        public override string TransactionId => "";

        public override string TransactionStatus => "";


    }
}
