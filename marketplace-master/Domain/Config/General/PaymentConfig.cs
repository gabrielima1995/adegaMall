using Application.Config.Common;
using Application.Config.General.Integrator;
using Domain.Config.General;
using Domain.Config.General.Integrator;
using Newtonsoft.Json;
using System;

namespace Application.Config.General
{
    public class PaymentConfig : StatusConfig
    {
        private const string iugu = "iugu";
        private const string pagarme = "pagarme";
        private const string stripe = "stripe";
        private const string wirecard = "moip";

        [JsonIgnore]
        public GatewayConfig PaymentGateway
        {
            get
            {
                switch (ActiveGateway.ToLower())
                {
                    case wirecard:
                        return Moip;
                    case iugu:
                        throw new NotImplementedException();
                    case pagarme:
                    default: // Compatibilidade
                        return PagarMe;
                }
            }
        }

        public PagarMeIntegrator PagarMe { get; set; } = new PagarMeIntegrator();
        public PaymentCommonConfig Common { get; set; } = new PaymentCommonConfig();
        public IuguConfig Iugu { get; set; } = new IuguConfig();
        public MoipConfig Moip { get; set; } = new MoipConfig();
        public StripeConfig Stripe { get; set; } = new StripeConfig();
        public StatusConfig AntiFraud { get; set; } = new StatusConfig(enabled: false);
        public string ActiveGateway { get; set; } = pagarme;

        public bool GatewayIsPagarme() => ActiveGateway == pagarme;
        public bool GatewayIsWirecard() => ActiveGateway == wirecard;
        public bool GatewayIsStripe() => ActiveGateway == stripe;

        [JsonIgnore]
        public bool SellerReceivesAnyPayment => PagarMe.SellerReceivesPagarmePayment || Moip.SellerReceivesMoipPayment;
    }
}