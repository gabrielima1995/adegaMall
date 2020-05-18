using Application.Config.Common;

namespace Domain.Config.Modules.Product
{
    public class NoShippingConfig : StatusConfig
    {
        public DeliveryTimeMessageConfig DeliveryTimeMessage { get; set; } = new DeliveryTimeMessageConfig(false, "NO_SHIPPING_DELIVERY_TIME");
    }
}
