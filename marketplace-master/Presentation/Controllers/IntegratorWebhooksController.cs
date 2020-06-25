using Application.Modules.Subscriptions;
using Application.Modules.Subscriptions.PagarMe;
using Application.Modules.Subscriptions.Stripe;
using Domain.DTO;
using Domain.Modules.Subscriptions;
using Domain.Services.StoreModule;
using Hangfire;
using NewIntegrator.Stripe.Subscriptions;
using Newtonsoft.Json;
using PaymentIntegrator.Core.Subscriptions;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using V2.Application.Modules.Subscriptions;

namespace Presentation.Controllers
{
    public class IntegratorWebhooksController : Controller
    {
        protected OrderService OrderService { get; }

        public IntegratorWebhooksController(OrderService orderService)
        {
            OrderService = orderService;
        }

        protected string GetFormContent()
        {
            if (Request.TotalBytes > 0)
                return Encoding.Default.GetString(Request.BinaryRead(Request.TotalBytes));

            return string.Empty;
        }
    }

    public class IuguController : IntegratorWebhooksController
    {
        public IuguController(OrderService orderService) : base(orderService)
        {
        }

        [HttpPost]
        public ActionResult BoletoPostback()
        {
            var postContent = GetFormContent();
            var postback = new PaymentIntegrator.Iugu.Webhooks.Postback(postContent);
            var transactionPostback = new TransactionPostback(postback.Id, postContent);

            OrderService.ReceiveGatewayPostback(transactionPostback);
            return new HttpStatusCodeResult(200);
        }
    }

    public class PagarMeController : IntegratorWebhooksController
    {
        private readonly ISubscriptionGateway subscriptionGateway;

        public PagarMeController(OrderService orderService, ISubscriptionGateway subscriptionGateway) : base(orderService)
        {
            this.subscriptionGateway = subscriptionGateway;
        }

        [HttpPost]
        public ActionResult Notify(PagarMeSubscriptionPostback transaction)
        {
            JobHelper.Enqueue<PagarMeSubscriptionGateway>(i => i.ProcessPostback(transaction));
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult SubscriptionPostback()
        {
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var data = reader.ReadToEnd();
                    string query = HttpUtility.UrlDecode(data);
                    JobHelper.Enqueue<ISubscriptionGateway>(i => i.MapToProcessPostback(query));
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }

            return new HttpStatusCodeResult(200); /*OK!*/
        }

        [HttpPost]
        public ActionResult BoletoPostback()
        {
            // Exemplo de postback: http://puu.sh/hdPWZ/fe35cb7980.png
            using (var reader = new StreamReader(Request.InputStream))
            {
                var transactionId = Request.Form["id"];
                var formData = reader.ReadToEnd();

                OrderService.ReceiveGatewayPostback(new TransactionPostback(transactionId, formData));
                return new HttpStatusCodeResult(200);
            }
        }
    }

    public class MoipController : IntegratorWebhooksController
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly ISubscriptionOperations _subscriptionOperations;

        public MoipController(
            SubscriptionService subscriptionService,
            ISubscriptionOperations subscriptionOperations,
            OrderService service) : base(service)
        {
            _subscriptionService = subscriptionService;
            _subscriptionOperations = subscriptionOperations;
        }

        [HttpPost]
        public ActionResult TransactionPostback()
        {
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var data = reader.ReadToEnd();

                    dynamic requestData = JsonConvert.DeserializeObject(data);
                    var transactionId = requestData.resource.multiorder.multiPayment[0].id;

                    OrderService.ReceiveGatewayPostback(new TransactionPostback((string)transactionId, data));
                    return new HttpStatusCodeResult(200);
                }
            }
            catch (Exception e)
            {
                Helpers.LogHelper.LogString(e.Message);
                throw;
            }
        }

        [HttpPost]
        public ActionResult BoletoPostback() => TransactionPostback();

        [HttpPost]
        public ActionResult SubscriptionPostback()
        {
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var data = reader.ReadToEnd();
                    Helpers.LogHelper.LogString(data);

                    dynamic eventData = JsonConvert.DeserializeObject(data);
                    switch (((string)eventData.@event).ToLowerInvariant())
                    {
                        case "subscription.updated":
                        case "subscription.canceled":
                        case "subscription.suspended":
                        case "subscription.activated":
                            UpdateSubscription(eventData);
                            break;

                        default:
                            return new HttpStatusCodeResult(200);
                    }
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(200);
            }

            return new HttpStatusCodeResult(200); /*OK!*/
        }

        private void UpdateSubscription(dynamic subscriptionData)
        {
            string id = (string)subscriptionData.resource.code;
            var status = _subscriptionOperations.GetSubscriptionStatus(_subscriptionService.Get(x => x.ReferenceId == id)).Result;

            //var subscription = _subscriptionService.Get(x => x.GatewayId == id);
            //subscription.SetCurrentStatus(GetStatus(data.status));
            //_subscriptionService.Save(x => x.ReferenceId == id, () => null, subs => subs.SetCurrentStatus(GetStatus((string)subscriptionData.resource.status)));
            _subscriptionService.Save(x => x.ReferenceId == id, () => null, subs => subs.SetCurrentStatus(status));
        }

        private void RegisterInvoice(dynamic invoiceData, string rawData)
        {
            string id = (string)invoiceData.resource.subscription_code;
            string invoiceId = (string)invoiceData.resource.id;
            string invoiceStatus = (string)invoiceData.resource.status.code;

            _subscriptionService.Save(x => x.GatewayId == id, () => null, subs =>
            {
                subs.AddTransaction(invoiceId, invoiceStatus, rawData, id);
            });
        }

        private SubscriptionStatus GetStatus(string status)
        {
            switch (status.ToUpperInvariant())
            {
                case "SUSPENDED":
                case "EXPIRED":
                    return SubscriptionStatus.Ended;

                case "ACTIVE":
                    return SubscriptionStatus.Paid;

                case "TRIAL":
                    return SubscriptionStatus.Trialing;

                case "CANCELED":
                    return SubscriptionStatus.Canceled;

                case "OVERDUE":
                default:
                    return SubscriptionStatus.Unpaid;
            }
        }
    }

    [RoutePrefix("stripe")]
    public class StripeController : IntegratorWebhooksController
    {
        private readonly StripeSubscriptionGateway _gateway;

        public StripeController(OrderService orderService, StripeSubscriptionGateway gateway) : base(orderService)
        {
            _gateway = gateway;
        }

        [HttpPost]
        [Route("subscription/invoice/paymentsucceeded")]
        public ActionResult SubscriptionInvoicePaymentSucceeded(StripeWebhookRoot<StripeInvoiceWebhookObject> @event)
        {
            var service = new StripeSubscriptionService();

            var subscription = service.Get(@event.data.@object.subscription);
            var postback = new StripeSubscriptionPostback(subscription);

            var result = _gateway.ProcessPostback(postback);

            if (result.OK)
            {
                JobHelper.Schedule<SubscriptionV2AppService>(i => i.CheckCredits(result.Result.SellerId), TimeSpan.FromMinutes(2)); // para nao dar conflito com outros webhooks similares
            }

            return new EmptyResult();
        }

        [HttpPost]
        [Route("subscription/statuschanged")]
        public ActionResult SubscriptionStatusChanged(StripeWebhookRoot<StripeSubscriptionWebhookObject> @event)
        {
            var postback = new StripeSubscriptionWebhookPostback(@event.data.@object);

            var result = _gateway.ProcessPostback(postback);

            if (result.OK)
            {
                JobHelper.Schedule<SubscriptionV2AppService>(i => i.CheckCredits(result.Result.SellerId), TimeSpan.FromMinutes(1)); // para nao dar conflito com outros webhooks similares
            }

            return new EmptyResult();
        }
    }
}