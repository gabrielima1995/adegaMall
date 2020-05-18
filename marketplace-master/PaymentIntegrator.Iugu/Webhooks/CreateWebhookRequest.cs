using Framework;
using PaymentIntegrator.Iugu.Common;

namespace PaymentIntegrator.Iugu.Webhooks
{
    public class CreateWebhookRequest : IValidatable
    {
        public EventType Event { get; set; }

        public string Url { get; set; }

        public string Authorization { get; set; }

        public CreateWebhookRequest()
        {
        }

        public CreateWebhookRequest(EventType eventType, string url) : this(eventType, url, null)
        {
        }

        public CreateWebhookRequest(EventType eventType, string url, string authorization)
        {
            Event = eventType;
            Url = url;
            Authorization = authorization;
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (Url == null)
                errors.Add(nameof(Url), "Url cannot be null.");

            return errors;
        }
    }

}
