using Domain.Entities;
using System;

namespace Domain.Modules.Finance
{
    public class GatewayRegistrationLog : BaseEntity
    {
        private const string pagarme = "pagarme";
        private const string wirecard = "wirecard";

        private GatewayRegistrationLog(Guid id, string sellerId, string gateway, string gatewayRecipientId, string gatewayBankAccountId, string gatewayApiToken, string extraInfoJson)
        {
            if (string.IsNullOrEmpty(sellerId))
                throw new ArgumentNullException(nameof(sellerId));

            if (string.IsNullOrEmpty(gatewayRecipientId))
                throw new ArgumentNullException(nameof(gatewayRecipientId));

            if (string.IsNullOrEmpty(gatewayBankAccountId))
                throw new ArgumentNullException(nameof(gatewayBankAccountId));

            if (string.IsNullOrEmpty(extraInfoJson))
                throw new ArgumentNullException(nameof(extraInfoJson));

            Id = id;
            SellerId = sellerId;
            Gateway = gateway;
            GatewayRecipientId = gatewayRecipientId;
            GatewayBankAccountId = gatewayBankAccountId;
            GatewayApiToken = gatewayApiToken;
            ExtraInfo = extraInfoJson;
        }

        public Guid Id { get; private set; }
        public string SellerId { get; private set; }
        public string Gateway { get; private set; }
        public string GatewayRecipientId { get; private set; }
        public string GatewayBankAccountId { get; private set; }
        public string GatewayApiToken { get; private set; }
        public string ExtraInfo { get; private set; }

        public bool IsPagarme => Gateway == pagarme;
        public bool IsWirecard => Gateway == wirecard;

        public static GatewayRegistrationLog PagarmeLog(Guid id, string sellerId, string pagarmeRecipientId, string pagarmeBankAccountId, string extraInfoJson)
        {
            return new GatewayRegistrationLog(id, sellerId, pagarme, pagarmeRecipientId, pagarmeBankAccountId, null, extraInfoJson);
        }

        public static GatewayRegistrationLog WirecardLog(Guid id, string sellerId, string wirecardRecipientId, string wirecardBankAccountId, string apiToken, string extraInfoJson)
        {
            if (string.IsNullOrEmpty(sellerId))
                throw new ArgumentNullException(nameof(apiToken));

            if (!wirecardRecipientId.StartsWith("MPA-"))
                throw new ArgumentException("O identificador da conta Wirecard deve estar no formato MPA-XXXXXXXXXXXX.");

            if (!wirecardBankAccountId.StartsWith("BKA-"))
                throw new ArgumentException("O identificador da conta bancária Wirecard deve estar no formato BKA-XXXXXXXXXXXX.");

            return new GatewayRegistrationLog(id, sellerId, wirecard, wirecardRecipientId, wirecardBankAccountId, apiToken, extraInfoJson);
        }

        public override object GetId() => Id;
    }
}
