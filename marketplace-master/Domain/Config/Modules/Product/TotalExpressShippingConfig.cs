using Application.Config.Common;
using Framework.Settings;
using System;

namespace Domain.Config.Modules.Product
{
    public class TotalExpressShippingConfig : StatusConfig
    {
        public DeliveryTimeMessageConfig DeliveryTimeMessage { get; set; } = new DeliveryTimeMessageConfig(true, "ASAPLOG_DELIVERY_TIME");

        public EndpointSettings ConsultaFrete { get; set; } = new EndpointSettings
        {
            Key = "TotalExpress.ConsultaFrete",
            Url = "https://edi.totalexpress.com.br/webservice_calculo_frete.php?wsdl",
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

        public string TipoServico { get; set; } = "EXP";
    }
}