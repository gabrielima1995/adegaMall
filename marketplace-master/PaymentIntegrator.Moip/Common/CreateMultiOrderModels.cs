using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Moip.Orders;
using PaymentIntegrator.Moip.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PaymentIntegrator.Moip.Common
{
    public class CreateMultiOrderRequest : MoipRequest
    {
        private CreateMultiOrderRequest(object multiOrderData)
            : base("multiorders", Method.POST)
        {
            AddParameter(@"application/json", PaymentIntegrator.Moip.Utils.JsonHelper.Serialize(multiOrderData), ParameterType.RequestBody);
        }

        public static CreateMultiOrderRequest Create(IList<IOrder> orders, ICustomer customer, IPaymentOptions paymentOptions)
        {
            if (orders == null)
                throw new ArgumentNullException(nameof(orders));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (paymentOptions == null)
                throw new ArgumentNullException(nameof(paymentOptions));

            var multiOrderData = new
            {
                ownId = Guid.NewGuid(),
                orders = orders.Select(x => CreateOrder(x, customer, paymentOptions)).ToArray(),
            };

            return new CreateMultiOrderRequest(multiOrderData);
        }

        private static object CreateOrder(IOrder order, ICustomer customer, IPaymentOptions paymentOptions)
        {
            var orderData = new
            {
                ownId = order.OrderId,
                amount = new
                {
                    currency = "BRL",
                    subtotals = new
                    {
                        shipping = order.Shipping.ToCents(),
                        addition = order.InstallmentFees.ToCents(),
                    },
                },
                items = order.Items.Select(x => new
                {
                    product = x.Item,
                    quantity = x.Quantity,
                    detail = x.Item,
                    price = x.Price.ToCents(),
                }),
                customer = new
                {
                    id = customer.GatewayId,
                },
                receivers = CreateReceivers(order),
            };

            return orderData;
        }

        private static MoipReceiver[] CreateReceivers(IOrder order)
        {
            order.SetProcessingGatewayFee(MoipConfig.Current.RecipientChargeProcessingFee);
           
            var startupAmount = order.GetSplitFees();
            var receivers = new MoipReceiver[2];
            var sellerAmount = order.Amount - startupAmount;

            if (MoipConfig.Current.MarketplaceZeroCommission)
            {
                receivers[0] = MoipReceiver.WithPercentualAmount("PRIMARY", false, MoipConfig.Current.StartupReceiverId, 0);
                receivers[1] = MoipReceiver.WithPercentualAmount("SECONDARY", true, order.Supplier.GatewayId, 100);
            }
            else
            {
                receivers[0] = MoipReceiver.WithFixedAmount("PRIMARY", !order.ProcessingGatewayFee, MoipConfig.Current.StartupReceiverId, startupAmount.ToCents());
                receivers[1] = MoipReceiver.WithFixedAmount("SECONDARY", order.ProcessingGatewayFee, order.Supplier.GatewayId, sellerAmount.ToCents());
            }

            return receivers;
        }
    }

    public class CreateMultiOrderResult : MoipResponse
    {
        private CreateMultiOrderResult(IRestResponse restResponse, CreateMultiOrderResultData data) : base(restResponse)
        {
            Data = data;
        }

        public static CreateMultiOrderResult Create(IRestResponse restResponse)
        {
            var data = PaymentIntegrator.Moip.Utils.JsonHelper.Deserialize<CreateMultiOrderResultData>(restResponse.Content);
            return new CreateMultiOrderResult(restResponse, data);
        }

        public CreateMultiOrderResultData Data { get; }
        protected override HttpStatusCode[] SuccessfulStatusCodes => new[] { HttpStatusCode.OK, HttpStatusCode.Created };

        public class CreateMultiOrderResultData
        {
            public string Id { get; private set; }
        }
    }
}
