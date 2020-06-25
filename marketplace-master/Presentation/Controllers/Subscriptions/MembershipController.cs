using Application.Config;
using Application.CreditCards;
using Application.Modules.Subscriptions;
using Application.Sellers;
using Domain.Modules.Subscriptions.Plans;
using Domain.Sellers;
using Framework;
using NewIntegrator.Stripe.Customers;
using NewIntegrator.Stripe.Invoices;
using NewIntegrator.Stripe.PaymentIntents;
using NewIntegrator.Stripe.Subscriptions;
using Presentation.Globalization;
using Presentation.Models.SubscriptionViewModels;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [AuthorizeSeller]
    public class MembershipController : BaseController
    {
        private const string ViewPathFmt = "~/Views/Subscriptions/{0}.cshtml";

        private readonly SellerService_Obsolete _sellerService;
        private readonly PlanService _planService;
        private readonly SubscriptionAppService _subscriptionAppService;
        private readonly SellerAppService _sellerAppService;

        public MembershipController(
            SellerService_Obsolete sellerService,
            PlanService planService,
            SubscriptionAppService subscriptionAppService,
            SellerAppService sellerAppService)
        {
            _sellerService = sellerService;
            _planService = planService;
            _subscriptionAppService = subscriptionAppService;
            _sellerAppService = sellerAppService;
        }

        public ActionResult Index()
        {
            var config = MaaSConfig.Current;
            var sellerId = GetPersonId();
            var seller = _sellerService.Get(i => i.Id == sellerId, i => i.Subscription);

            if (seller.IsValidSubscription || !config.Subscription.Enabled)
                return RedirectToAction("Index", "Manage", new { lang = Request.GetCurrentLang() });

            _sellerAppService.StartSubscription(sellerId);

            var model = _subscriptionAppService.GetSignModesList(seller, Request.GetCurrentLang());
            return View(string.Format(ViewPathFmt, "Membership"), model);
        }

        public ActionResult Subscribe(Guid id, string lang)
        {
            var model = _subscriptionAppService.PrepareSubscribe(id, lang);
            var sellerId = GetPersonId();
            var seller = _sellerService.Get(i => i.Id == sellerId, x => x.Address);

            if (seller.Address == null || seller.Phone == null)
                this.AddErrors(new OperationResult("", "Complete seu cadastro antes de comprar a assinatura. Informe todos os dados, incluindo endereço completo e telefone. Esses dados são validados pela instituição bancária no ato do pagamento"));

            if (model == null) return this.NotFound();

            return View(string.Format(ViewPathFmt, "Subscribe"), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Subscribe(Guid id, CreditCardModel creditCard, string lang)
        {
            var sellerId = GetPersonId();
            var seller = _sellerService.Get(i => i.Id == sellerId, x => x.Address);

            if (seller.Address == null || seller.Phone == null)
                this.AddErrors(new OperationResult("", "Complete seu cadastro antes de comprar a assinatura. Informe todos os dados, incluindo endereço completo e telefone. Esses dados são validados pela instituição bancária no ato do pagamento"));

            if (ModelState.IsValid)
            {
                var validation = _subscriptionAppService.Subscribe(sellerId, id, creditCard);

                if (validation.OK) return View(string.Format(ViewPathFmt, "SubscribeConfirmation"));

                this.AddErrors(validation, true);
            }

            var model = _subscriptionAppService.PrepareSubscribe(id, lang);
            if (model == null) return this.NotFound();

            model.CreditCard = creditCard;

            return View(string.Format(ViewPathFmt, "Subscribe"), model);
        }

        [HttpPost]
        public async Task<ActionResult> SubscribeStripe(Guid id, StripeSubscriptionRequest model, string lang)
        {
            var sellerId = GetPersonId();

            if (ModelState.IsValid)
            {
                _sellerAppService.RegisterGateway(sellerId);

                StripePaymentIntent intent = null;
                StripeInvoice invoice = null;

                var paymentIntentService = new StripePaymentIntentService();
                var customerService = new StripeCustomerService();
                var subscriptionService = new StripeSubscriptionService();
                var invoiceService = new StripeInvoiceService();

                var seller = _sellerService.Get(i => i.Id == sellerId);

                if (string.IsNullOrEmpty(seller.CustomerGatewayId))
                {
                    return Json(new
                    {
                        success = false,
                        error = "Fail to create customer in Stripe"
                    });
                }

                var plan = _planService.Get(i => i.Id == id);

                if (plan == null || !plan.Enabled)
                {
                    return Json(new
                    {
                        success = false,
                        error = "Invalid plan."
                    });
                }

                try
                {
                    // Cria assinatura no gateway
                    if (model.card_token != null)
                    {
                        // vincula cartão ao cliente
                        await customerService.ChangeSourceAsync(seller.CustomerGatewayId, model.card_token);

                        var subscription = subscriptionService.GetByCustomer(seller.CustomerGatewayId);

                        // cria assinatura
                        if (subscription == null)
                        {
                            subscription = await subscriptionService.CreateAsync(seller.CustomerGatewayId, plan.GatewayId);
                            invoice = subscription.LastInvoice;
                        }
                        // retenta pagar a invoice
                        else if (!subscription.LastInvoice.Paid)
                        {
                            invoice = await invoiceService.PayAsync(subscription.LastInvoice.Id);
                        }
                        else
                        {
                            invoice = subscription.LastInvoice;
                        }

                        intent = invoice.PaymentIntent;
                    }
                    // Obtem intenção de pagamento
                    else
                    {
                        intent = await paymentIntentService.GetAsync(model.payment_intent_id);
                    }
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        success = false,
                        error = e.Message
                    });
                }

                if (intent?.Status == StripePaymentIntentStatus.RequiresPaymentMethod)
                {
                    return Json(new
                    {
                        success = false,
                        error = "Payment fails. Enter a new credit card."
                    });
                }

                if (intent?.Status == StripePaymentIntentStatus.RequiresAction)
                {
                    // Tell the client to handle the action
                    return Json(new
                    {
                        requires_action = true,
                        payment_intent_client_secret = intent.ClientSecret,
                        payment_intent_id = intent.Id
                    });
                }

                if (intent?.Status == StripePaymentIntentStatus.Succeeded || (invoice != null && invoice.Paid))
                {
                    // The payment didn’t need any additional actions and completed!
                    // Handle post-payment fulfillment
                    var result = _subscriptionAppService.Subscribe(sellerId, id, null);

                    if (result.OK)
                    {
                        return Json(new
                        {
                            success = true,
                            html = RenderRazorViewToString(string.Format(ViewPathFmt, "SubscribeConfirmation"), null)
                        });
                    }

                    return Json(new
                    {
                        success = false,
                        error = result.Errors.ToSimpleString()
                    });
                }

                // Invalid status
                return Json(new
                {
                    success = false,
                    error = "Invalid PaymentIntent status"
                });
            }

            // Invalid status
            return Json(new
            {
                success = false,
                error = "Invalid model"
            });
        }
    }
}