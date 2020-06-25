using Application.Config;
using Application.CreditCards;
using Application.Sellers;
using Domain.Entities.SubscriptionModule;
using Domain.Interfaces;
using Domain.Modules.Subscriptions;
using Domain.Sellers;
using Framework;
using PaymentIntegrator.Core.Subscriptions;
using System.Linq;

namespace Application.Modules.Subscriptions.PagarMe
{
    /// <summary>
    /// Representa operações de assinatura para o gateway de pagamento Pagar.me.
    /// </summary>
    public class PagarMeSubscriptionGateway : SubscriptionGatewayBase<PagarMeSubscriptionPostback>, ISubscriptionGateway
    {
        private readonly SellerService_Obsolete _sellerService;
        private readonly SellerAppService _sellerAppService;
        private readonly PagarmePostbackHelper _pagarmePostbackHelper;
        private readonly ISubscriptionOperations _subscriptionOperations;

        public string NotifyUrl => MaaSConfig.Current.Environment.AppUrl + "pagarme/subscriptionpostback/";

        public PagarMeSubscriptionGateway(
            SubscriptionService subscriptionService,
            SellerService_Obsolete sellerService,
            IEmailSender emailSender,
            SellerAppService sellerAppService,
            PagarmePostbackHelper pagarmePostbackHelper,
            ISubscriptionOperations subscriptionOperations,
            IRepository<Subscription> subscriptionRepository)
            : base(subscriptionService, subscriptionRepository)
        {
            _sellerService = sellerService;
            _sellerAppService = sellerAppService;
            _pagarmePostbackHelper = pagarmePostbackHelper;
            _subscriptionOperations = subscriptionOperations;
        }

        public override OperationResult<Subscription> ProcessPostback(PagarMeSubscriptionPostback postback)
        {
            var seller = _sellerService.Get(
                i => i.Subscription.ReferenceId == postback.id,
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
            var user = _sellerService.Get(i => i.Id == personId,
                i => i.Subscription,
                i => i.Subscription.Plan,
                i => i.Address);

            if (user.CustomerGatewayId == null)
                _sellerAppService.RegisterGateway(personId);

            var subsValidation = _subscriptionOperations.Subscribe(plan, creditCard, user, user.Address, NotifyUrl);

            if (subsValidation.OK)
            {
                // Cria ou altera a assinatura no banco de dados
                return CreateOrUpdateSubscription(new PagarMeSubscriptionPostback(subsValidation.Result), user, plan, true);
            }

            return new OperationResult<Subscription>(subsValidation.Errors);
        }

        public OperationResult<Subscription> Unsubscribe(string personId)
        {
            var seller = _sellerService.Get(i => i.Id == personId,
                i => i.Subscription,
                i => i.Subscription.Plan);

            if (seller.IsValidSubscription)
            {
                var validation = _subscriptionOperations.Unsubscribe(seller.Subscription);

                if (validation.OK)
                    return CancelSubscription(seller.Subscription);

                return new OperationResult<Subscription>(validation.Errors);
            }

            return new OperationResult<Subscription>();
        }

        public OperationResult MapToProcessPostback(string postbackQuery)
        {
            var resultMap = _pagarmePostbackHelper.MapPostbackToSubscriptionPostbackData(postbackQuery);
            if (resultMap.IsFailure)
                throw new System.Exception(resultMap.Errors.First().Value);
            return ProcessPostback(resultMap.Result);
        }
    }
}