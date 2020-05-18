using Framework;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;

namespace PaymentIntegrator.Moip.Payments
{
    public class MoipPaymentMethodOperations : IPaymentMethodOperations
    {
        public OperationResult<ICreditCardResult> CreateCreditCard(ICustomer customer, ICreditCard creditCard, bool isSubscribe = false)
        {
            //Moip does not allow early storage of credit cards for non-PCI-Compliant services
            return new OperationResult<ICreditCardResult>(result: null);

            //var createCardRequest = new MoipRequest($"customers/{customer.GatewayId}/fundinginstruments", RestSharp.Method.POST);
            //var cardObject = new
            //{
            //    method = "CREDIT_CARD",
            //    creditCart = new
            //    {

            //    }
            //};
        }
    }
}
