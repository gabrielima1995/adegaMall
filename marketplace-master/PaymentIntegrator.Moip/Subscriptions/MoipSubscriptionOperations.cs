using Framework;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Common;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Core.Plans;
using PaymentIntegrator.Core.Subscriptions;
using PaymentIntegrator.Moip.Common;
using System;
using System.Net;
using System.Web;

namespace PaymentIntegrator.Moip.Subscriptions
{
    public class MoipSubscriptionOperations : ISubscriptionOperations
    {
        public OperationResult<SubscriptionStatus> GetSubscriptionStatus(ISubscription subscription)
        {
            var getStatusRequest = new MoipRequest($"subscriptions/{subscription.GatewayId}", RestSharp.Method.GET);
            var getStatusResponse = MoipClient.SendV1Request(getStatusRequest);
            if (getStatusResponse.StatusCode == HttpStatusCode.OK)
            {
                dynamic subscriptionData = JsonConvert.DeserializeObject(getStatusResponse.Content);
                string responseStatus = (string)subscriptionData.status;

                return MoipSubscription.GetStatus(responseStatus);
            }
            else
            {
                throw new Exception("Error reading current status");
            }
        }

        public OperationResult<ISubscription> Subscribe(IPlan plan, ICreditCard creditCard, ICustomer customer, IAddress address, string postbackUrl)
        {
            if (customer.GatewayId == null)
                throw new Exception("User has no registed customer id");

            //Create subscriber ID if customer doesn't have one
            if (string.IsNullOrWhiteSpace(customer.SubscriberId))
            {
                var createSubscriberRequest = new MoipRequest("customers", RestSharp.Method.POST);

                createSubscriberRequest.AddJsonBody(MoipSubscriber.CreateSubscriber(customer, address));
                var createSubscriberResponse = MoipClient.SendV1Request(createSubscriberRequest);

                if (createSubscriberResponse.StatusCode == HttpStatusCode.Created || createSubscriberResponse.StatusCode == HttpStatusCode.OK)
                {
                    customer.SetSubscriberId(customer.GatewayId);
                }
                else throw new HttpException((int)createSubscriberResponse.StatusCode, createSubscriberResponse.StatusCode.ToString());
            }

            //Update subscriber with credit card info provided
            var updateCardRequest = new MoipRequest($"customers/{customer.SubscriberId}/billing_infos", RestSharp.Method.PUT);
            updateCardRequest.AddJsonBody(new
            {
                credit_card = new
                {
                    holder_name = creditCard.FullName,
                    number = creditCard.Number.Replace(" ", "").Replace("-", "").Replace(".", ""),
                    expiration_month = creditCard.Month,
                    expiration_year = creditCard.Year.ToString().Substring(2, 2),
                }
            });
            var updateCardResponse = MoipClient.SendV1Request(updateCardRequest);
            if (updateCardResponse.StatusCode != HttpStatusCode.OK)
                throw new HttpException((int)updateCardResponse.StatusCode, updateCardResponse.StatusCode.ToString());

            //Create subscription
            var subscribeRequest = new MoipRequest($"subscriptions/?new_customer=false", RestSharp.Method.POST);
            subscribeRequest.AddJsonBody(new
            {
                code = (customer.SubscriberId + Guid.NewGuid().Substring(8)),
                payment_method = "CREDIT_CARD",
                plan = new
                {
                    code = plan.GatewayId,
                },
                customer = new
                {
                    code = customer.SubscriberId,
                }
            });
            var subscribeResponse = MoipClient.SendV1Request(subscribeRequest);
            if (subscribeResponse.StatusCode == HttpStatusCode.Created)
            {
                dynamic createdSubscription = JsonConvert.DeserializeObject(subscribeResponse.Content);

                var status = createdSubscription.status;

                var newSubscription = new MoipSubscription()
                {
                    Customer = customer,
                    Plan = plan,
                    Status = SubscriptionStatus.Unpaid,
                    GatewayId = createdSubscription.code,
                };

                newSubscription.SetCurrentStatus((string)status);
                if (newSubscription.Status == SubscriptionStatus.Paid || newSubscription.Status == SubscriptionStatus.Trialing)
                    return newSubscription;
            }

            return OperationResult<ISubscription>.Fail("Error", "Erro ao criar assinatura. Verifique seus dados de cadastro e do cartão, e tente novamente");
        }

        public OperationResult<ISubscription> Subscribe(IPlan plan, ICreditCardResult creditCardStatus, ICustomer customer, IAddress address, string postbackUrl)
        {
            throw new NotImplementedException();
        }

        public OperationResult<ISubscription> Unsubscribe(ISubscription subscription)
        {
            var cancelSubscriptionRequest = new MoipRequest($"subscriptions/{subscription.GatewayId}/cancel", RestSharp.Method.PUT);
            cancelSubscriptionRequest.AddJsonBody(new { });
            var cancelSubscriptionResponse = MoipClient.SendV1Request(cancelSubscriptionRequest);

            if (cancelSubscriptionResponse.StatusCode == HttpStatusCode.OK)
            {
                subscription.SetCurrentStatus(SubscriptionStatus.Canceled);
                return new OperationResult<ISubscription>(subscription);
            }

            throw new Exception("Cancelation failed");
        }
    }
}