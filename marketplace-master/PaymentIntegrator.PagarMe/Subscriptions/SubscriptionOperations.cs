using Framework;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Plans;
using PaymentIntegrator.Core.Subscriptions;

namespace PaymentIntegrator.PagarMe.Subscriptions
{
    public class SubscriptionOperations : ISubscriptionOperations
    {
        private readonly IPaymentMethodOperations _paymentMethodOperations;

        public SubscriptionOperations(IPaymentMethodOperations paymentMethodOperations)
        {
            _paymentMethodOperations = paymentMethodOperations;
        }

        public OperationResult<SubscriptionStatus> GetSubscriptionStatus(ISubscription subscription)
        {
            try
            {
                var result = new PagarMeClient().GetService().Subscriptions.Find(int.Parse(subscription.GatewayId), true);
                switch (result.Status)
                {
                    case global::PagarMe.SubscriptionStatus.None:
                        return SubscriptionStatus.None;
                    case global::PagarMe.SubscriptionStatus.Trialing:
                        return SubscriptionStatus.Trialing;
                    case global::PagarMe.SubscriptionStatus.Paid:
                        return SubscriptionStatus.Paid;
                    case global::PagarMe.SubscriptionStatus.PendingPayment:
                        return SubscriptionStatus.PendingPayment;
                    case global::PagarMe.SubscriptionStatus.Unpaid:
                        return SubscriptionStatus.Unpaid;
                    case global::PagarMe.SubscriptionStatus.Canceled:
                        return SubscriptionStatus.Canceled;
                    case global::PagarMe.SubscriptionStatus.Ended:
                        return SubscriptionStatus.Ended;
                    default:
                        break;
                }
            }
            catch
            {

            }

            return OperationResult<SubscriptionStatus>.Fail("error", "Unable to retrieve subscription status");
        }

        public OperationResult<ISubscription> Subscribe(IPlan plan, ICreditCard creditCard,
            ICustomer customer, IAddress address, string postbackUrl)
        {
            var validation = _paymentMethodOperations.CreateCreditCard(customer, creditCard, true);

            if (validation.OK)
            {
                return Subscribe(plan, validation.Result, customer, address, postbackUrl);
            }

            return new OperationResult<ISubscription>(validation.Errors);
        }

        public OperationResult<ISubscription> Subscribe(IPlan plan, ICreditCardResult creditCardStatus,
            ICustomer customer, IAddress address, string postbackUrl)
        {
            var client = new PagarMeClient();

            var request = new SubscriptionRequest(client, plan, customer, address, creditCardStatus, postbackUrl);

            var validationRequest = request.Validate();

            if (validationRequest.HasErrors)
            {
                return new OperationResult<ISubscription>(validationRequest);
            }

            var validation = client.Save(request);

            if (validation.OK)
            {
                return new OperationResult<ISubscription>(new SubscriptionResponse(request));
            }

            return new OperationResult<ISubscription>(validation.Errors);
        }

        public OperationResult<ISubscription> Unsubscribe(ISubscription subscription)
        {
            var client = new PagarMeClient();

            var subscriptionCancel = client.GetService().Subscriptions.Find(subscription.GatewayId);

            var validation = client.Execute(() => subscriptionCancel.Cancel());

            if (validation.OK)
            {
                subscription.SetCurrentStatus((SubscriptionStatus)subscriptionCancel.Status.GetHashCode());

                return new OperationResult<ISubscription>(subscription);
            }

            return new OperationResult<ISubscription>(validation.Errors);
        }
    }
}