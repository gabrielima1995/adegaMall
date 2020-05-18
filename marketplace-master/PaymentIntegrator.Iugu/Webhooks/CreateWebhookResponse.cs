namespace PaymentIntegrator.Iugu.Webhooks
{
    public class CreateWebhookResponse
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string Authorization { get; set; }

        public EventType Event { get; set; }
    }
}
