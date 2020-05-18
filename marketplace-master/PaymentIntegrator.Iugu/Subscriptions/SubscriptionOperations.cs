using Framework;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Plans;
using PaymentIntegrator.Core.Subscriptions;
using System;

namespace PaymentIntegrator.Iugu.Subscriptions
{
    public class SubscriptionOperations : ISubscriptionOperations
    {
        public OperationResult<SubscriptionStatus> GetSubscriptionStatus(ISubscription subscription)
        {
            throw new NotImplementedException();
        }

        public OperationResult<ISubscription> Subscribe(IPlan plan, ICreditCard creditCard, ICustomer customer, IAddress address, string postbackUrl)
        {
            throw new NotImplementedException();
        }

        public OperationResult<ISubscription> Subscribe(IPlan plan, ICreditCardResult creditCardStatus, ICustomer customer, IAddress address,
            string postbackUrl)
        {
            throw new NotImplementedException();
        }

        public OperationResult<ISubscription> Unsubscribe(ISubscription subscription)
        {
            throw new NotImplementedException();
        }
    }
}