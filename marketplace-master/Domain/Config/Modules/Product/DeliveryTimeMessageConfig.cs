namespace Domain.Config.Modules.Product
{
    public class DeliveryTimeMessageConfig
    {
        public bool Enabled { get; set; }
        public string MessageLabel { get; set; }

        public DeliveryTimeMessageConfig()
        {
        }
        public DeliveryTimeMessageConfig(bool enabled, string messageLabel)
        {
            Enabled = enabled;
            MessageLabel = messageLabel;
        }
    }
}
