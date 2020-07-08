using Application.Config.Common;
using Framework.Settings;
using System;

namespace Domain.Config.Modules.Product
{
    public class AsapLogShippingConfig : StatusConfig
    {
        public DeliveryTimeMessageConfig DeliveryTimeMessage { get; set; } = new DeliveryTimeMessageConfig(true, "ASAPLOG_DELIVERY_TIME");

        public EndpointSettings ConsultaFrete { get; set; } = new EndpointSettings
        {
            Key = "AsapLog.ConsultaFrete",
            Url = "https://app.asaplog.com.br/webservices/v1/consultarFrete",
            Timeout = TimeSpan.Parse("00:00:30"),
            RetrySleeps = new[]
            {
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(4),
            },
            EventsBeforeBreaking = 5,
            DurationOfBreak = TimeSpan.FromSeconds(50)
        };
    }
}