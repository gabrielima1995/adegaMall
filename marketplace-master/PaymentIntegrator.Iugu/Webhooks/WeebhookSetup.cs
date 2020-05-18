using Framework;

namespace PaymentIntegrator.Iugu.Webhooks
{
    public static class WeebhookSetup
    {
        public static OperationResult Configure()
        {
            var boletoWebhookResult = CreateBoletoWebhook();
            if (!boletoWebhookResult.OK) return boletoWebhookResult.Errors;

            return OperationResult.Success;
        }

        private static OperationResult CreateBoletoWebhook()
        {
            if (IuguConfig.PostbackUrl == null)
                return new OperationResult("", "IuguConfig.PostbackUrl must point to a notifiable URI.");

            var webhookOperations = new WebhookOperations();
            var eventType = EventType.InvoiceStatusChanged;

            var listResult = webhookOperations.ListWebhooks();
            if (!listResult.OK) return listResult.Errors;

            foreach (var item in listResult.Result)
            {
                if (item.Event == eventType)
                {
                    if (item.Url != IuguConfig.PostbackUrl)
                    {
                        var alterResult = webhookOperations.AlterWebhook(item.Id, eventType, IuguConfig.PostbackUrl);
                        if (!alterResult.OK) return alterResult.Errors;
                    }

                    return OperationResult.Success;
                }
            }

            var createResult = webhookOperations.CreateWebhook(eventType, IuguConfig.PostbackUrl);
            if (!createResult.OK) return createResult.Errors;

            return OperationResult.Success;
        }
    }
}
