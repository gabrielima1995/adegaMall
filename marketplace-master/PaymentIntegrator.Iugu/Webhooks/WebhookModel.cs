namespace PaymentIntegrator.Iugu.Webhooks
{
    public class WebhookModel
    {
        public string Id { get; set; }

        public EventType Event { get; set; }

        public string Url { get; set; }

        public string Authorization { get; set; }

        public WebhookModel()
        {
        }

        public WebhookModel(string id, EventType eventType, string url) : this(id, eventType, url, null)
        {
        }

        public WebhookModel(string id, EventType eventType, string url, string authorization)
        {
            Id = id;
            Event = eventType;
            Url = url;
            Authorization = authorization;
        }
    }
}
