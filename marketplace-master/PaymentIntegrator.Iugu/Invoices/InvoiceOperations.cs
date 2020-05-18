using Framework;

namespace PaymentIntegrator.Iugu.Invoices
{
    public class InvoiceOperations
    {
        public OperationResult<GetInvoiceResponse> GetInvoice(string id)
        {
            var client = GetClient("/invoices/" + id);
            var getResult = client.Get<GetInvoiceResponse>();
            if (!getResult.OK) return getResult.Errors;

            return getResult;
        }

        private IuguClient GetClient(string resource)
        {
            var apiKey = IuguConfig.IsTest ? IuguConfig.ApiKeyTest : IuguConfig.ApiKeyLive;
            return new IuguClient(resource, apiKey);
        }
    }
}
