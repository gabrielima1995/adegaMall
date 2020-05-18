using Framework;
using System.Collections.Generic;

namespace PaymentIntegrator.Iugu.Webhooks
{
    public class WebhookOperations
    {
        public OperationResult AlterWebhook(string id, EventType eventType, string url)
        {
            var request = new AlterWebhookRequest(id, eventType, url);
            var requestValidationResult = request.Validate();
            if (requestValidationResult.HasErrors) return requestValidationResult;

            var client = GetClient($"/web_hooks/{id}");
            var putResult = client.Put<WebhookModel>(request);
            if (!putResult.OK) return putResult.Errors;

            return OperationResult.Success;
        }

        public OperationResult CreateWebhook(EventType eventType, string url)
        {
            var request = new CreateWebhookRequest(eventType, url);
            var requestValidationResult = request.Validate();
            if (requestValidationResult.HasErrors) return requestValidationResult;

            var client = GetClient("/web_hooks");
            var postResult = client.Post<CreateWebhookResponse>(request);
            if (!postResult.OK) return postResult.Errors;

            return OperationResult.Success;
        }

        public OperationResult<IEnumerable<WebhookModel>> ListWebhooks()
        {
            var client = GetClient("/web_hooks");
            var getResult = client.Get<IEnumerable<WebhookModel>>();
            if (!getResult.OK) return getResult.Errors;

            return getResult;
        }

        public OperationResult RemoveWebhook(string id)
        {
            var client = GetClient($"/web_hooks/{id}");
            var deleteResult = client.Delete();
            if (!deleteResult.OK) return deleteResult.Errors;

            return OperationResult.Success;
        }

        private IuguClient GetClient(string resource)
        {
            var apiKey = IuguConfig.IsTest ? IuguConfig.ApiKeyTest : IuguConfig.ApiKeyLive;
            return new IuguClient(resource, apiKey);
        }
    }
}
