using Application.Config.Common;

namespace Domain.Config.Modules.Product
{
    public class AsapLogShippingConfig : StatusConfig
    {
        public DeliveryTimeMessageConfig DeliveryTimeMessage { get; set; } = new DeliveryTimeMessageConfig(false, "POSTAL_REGULAR_DELIVERY_TIME");
    }
}
