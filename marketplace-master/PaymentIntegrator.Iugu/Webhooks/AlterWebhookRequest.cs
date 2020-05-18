using Framework;
using PaymentIntegrator.Iugu.Common;

namespace PaymentIntegrator.Iugu.Webhooks
{
    public class AlterWebhookRequest : WebhookModel, IValidatable
    {
        public AlterWebhookRequest()
        {
        }

        public AlterWebhookRequest(string id, EventType eventType, string url) : base(id, eventType, url)
        {
        }

        public AlterWebhookRequest(string id, EventType eventType, string url, string authorization) : base(id, eventType, url, authorization)
        {
        }

        public ErrorDictionary Validate()
        {
            var errors = new ErrorDictionary();

            if (Id == null)
                errors.Add(nameof(Id), "Id cannot be null.");

            if (Url == null)
                errors.Add(nameof(Url), "Url cannot be null.");

            return errors;
        }
    }
}
