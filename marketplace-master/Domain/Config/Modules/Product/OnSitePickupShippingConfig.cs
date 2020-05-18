using Application.Config.Common;

namespace Domain.Config.Modules.Product
{
    public class OnSitePickupShippingConfig : StatusConfig
    {
        public DeliveryTimeMessageConfig DeliveryTimeMessage { get; set; } = new DeliveryTimeMessageConfig(false, "ON_SITE_PICKUP_DELIVERY_TIME");
    }
}
