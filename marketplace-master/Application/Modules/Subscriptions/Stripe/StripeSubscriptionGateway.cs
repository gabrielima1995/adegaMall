using Application.Config;
using Application.CreditCards;
using Domain.Entities.SubscriptionModule;
using Domain.Modules.Subscriptions;
using Domain.Sellers;
using Framework;
using NewIntegrator.Stripe.Subscriptions;
using System;

namespace Application.Modules.Subscriptions.Stripe
{
    /// <summary>
    /// Representa operações de assinatura para o gateway de pagamento Stripe.
    /// </summary>
    public class StripeSubscriptionGateway : SubscriptionGatewayBase<StripeSubscriptionWebhookPostback>, ISubscriptionGateway
    {
        private readonly SellerService_Obsolete _sellerService;

        public string NotifyUrl => MaaSConfig.Current.Environment.AppUrl + "stripe/subscription/statuschanged";

        public StripeSubscriptionGateway(
            SubscriptionService subscriptionService,
            IRepository<Subscription> subscriptionRepository,
            SellerService_Obsolete sellerService)
            : base(subscriptionService, subscriptionRepository)
        {
            _sellerService = sellerService;
        }

        public override OperationResult<Subscription> ProcessPostback(StripeSubscriptionWebhookPostback postback)
        {
            return ProcessPostbackInternal(postback);
        }

        public OperationResult<Subscription> ProcessPostback(StripeSubscriptionPostback postback)
        {
            return ProcessPostbackInternal(postback);
        }

        private OperationResult<Subscription> ProcessPostbackInternal(SubscriptionPostbackBase postback)
        {
            var seller = _sellerService.Get(
                i => i.Subscription.ReferenceId == postback.GatewayId,
                i => i.Subscription,
                i => i.Subscription.Plan);

            if (seller != null)
            {
                return CreateOrUpdateSubscription(postback, seller, seller.Subscription?.Plan, false);
            }

            return null;
        }

        public OperationResult<Subscription> Subscribe(string personId, Plan plan, CreditCardModel creditCard)
        {
            var seller = _sellerService.Get(i => i.Id == personId,
                i => i.Subscription,
                i => i.Subscription.Plan);

            var stripeService = new StripeSubscriptionService();

            // Obtem a assinatura no Stripe
            var stripeSubscription = stripeService.GetByCustomer(seller.CustomerGatewayId);

            if (stripeSubscription != null)
            {
                // Cria ou altera a assinatura no banco de dados
                return CreateOrUpdateSubscription(new StripeSubscriptionPostback(stripeSubscription), seller, plan, true);
            }

            return new OperationResult<Subscription>("", "Error to get subscription in Stripe.");
        }

        public OperationResult<Subscription> Unsubscribe(string personId)
        {
            var seller = _sellerService.Get(i => i.Id == personId,
                i => i.Subscription,
                i => i.Subscription.Plan);

            if (seller.IsValidSubscription)
            {
                try
                {
                    var stripeService = new StripeSubscriptionService();

                    stripeService.Cancel(seller.Subscription.ReferenceId);
                }
                catch (Exception e)
                {
                    return new OperationResult<Subscription>("", e.Message);
                }

                return CancelSubscription(seller.Subscription);
            }

            return new OperationResult<Subscription>();
        }

        public OperationResult MapToProcessPostback(string postbackQuery)
        {
            throw new NotImplementedException();
        }
    }
}