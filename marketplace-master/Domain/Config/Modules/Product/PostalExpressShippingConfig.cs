using Application.Config.Common;

namespace Domain.Config.Modules.Product
{
    public class PostalExpressShippingConfig : StatusConfig
    {
        public DeliveryTimeMessageConfig DeliveryTimeMessage { get; set; } = new DeliveryTimeMessageConfig(false, "POSTAL_EXPRESS_DELIVERY_TIME");
    }
}
