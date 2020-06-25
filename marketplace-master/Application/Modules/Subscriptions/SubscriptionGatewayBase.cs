using Domain.Entities;
using Domain.Entities.SubscriptionModule;
using Domain.Modules.Subscriptions;
using Framework;

namespace Application.Modules.Subscriptions
{
    /// <summary>
    /// Representa operações de assinatura para um gateway de pagamento.
    /// </summary>
    public abstract class SubscriptionGatewayBase<TPostback>
        where TPostback : SubscriptionPostbackBase
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly IRepository<Subscription> _subscriptionRepository;
        private static readonly object _lock = new object();

        protected SubscriptionGatewayBase(
            SubscriptionService subscriptionService,
            IRepository<Subscription> subscriptionRepository)
        {
            _subscriptionService = subscriptionService;
            _subscriptionRepository = subscriptionRepository;
        }

        /// <summary>
        /// Processa os dados de postback enviado pelo gateway de pagamento.
        /// </summary>
        /// <param name="postback">Dados de alteração de assinatura.</param>
        /// <returns></returns>
        public abstract OperationResult<Subscription> ProcessPostback(TPostback postback);

        /// <summary>
        /// Cria ou atualiza uma assinatura a partir dos dados do postback enviado pelo gateway de pagamento.
        /// </summary>
        /// <param name="postback">Dados de alteração de assinatura.</param>
        /// <param name="seller">Dados do assinante.</param>
        /// <param name="plan">Dados do plano de assinatura.</param>
        /// <param name="isSignup">Indica se a transação é de adesão.</param>
        /// <returns></returns>
        protected OperationResult<Subscription> CreateOrUpdateSubscription(SubscriptionPostbackBase postback, Seller seller, Plan plan, bool isSignup)
        {
            Subscription subscription;

            lock (_lock)
            {
                // Se for transação de assinatura expirada, porém não há dados de assinatura para o assinante, ignora processo.
                if (postback.IsExpiredTransaction && !seller.IsValidSubscription) return null;

                subscription = _subscriptionService.Save(
                    i => i.Seller.Id == seller.Id,
                    () => postback.CreateSubscription(seller, plan),
                    subs => postback.UpdateSubscription(subs, plan));
            }

            return subscription;
        }

        /// <summary>
        /// Cancela uma assinatura.
        /// </summary>
        /// <param name="subscription">Dados da assinatura a ser cancelada.</param>
        /// <returns></returns>
        protected Subscription CancelSubscription(Subscription subscription)
        {
            subscription.Cancel();

            _subscriptionRepository.Update(subscription);
            _subscriptionRepository.SaveChanges();

            return subscription;
        }
    }
}