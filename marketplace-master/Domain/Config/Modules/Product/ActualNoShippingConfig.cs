using Application.Config.Common;

namespace Domain.Config.Modules.Product
{
    public class ActualNoShippingConfig : StatusConfig
    {
        public DeliveryTimeMessageConfig DeliveryTimeMessage { get; set; } = new DeliveryTimeMessageConfig(false, "ACTUAL_NO_SHIPPING_DELIVERY_TIME");
    }
}
