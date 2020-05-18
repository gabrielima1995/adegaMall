using Application.Config.Common;
using Domain.Config.Modules.Product;
using System.Collections.Generic;

namespace Application.Config.Modules.Product
{
    /// <summary>
    /// Configuração de entrega de frota própria/privada.
    /// </summary>
    public class PrivateFleetShippingConfig : StatusConfig
    {
        /// <summary>
        /// Parâmetros padrão de raio de entrega.
        /// </summary>
        public IList<PrivateFleetParameterConfig> DefaultParams { get; set; } = new List<PrivateFleetParameterConfig>
        {
            new PrivateFleetParameterConfig
            {
                Range = 5
            },
            new PrivateFleetParameterConfig
            {
                Range = 10
            },
            new PrivateFleetParameterConfig
            {
                Range = 20
            }
        };

        public bool AllowFreeShipping { get; set; }

        public DeliveryTimeMessageConfig DeliveryTimeMessage { get; set; } = new DeliveryTimeMessageConfig(false, "PRIVATE_FLEET_SHIPPING_DELIVERY_TIME");
    }
}