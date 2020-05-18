using Application.Config;
using PaymentIntegrator.Core.Customers;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Moip.Payments;
using PaymentIntegrator.Moip.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PaymentIntegrator.Moip.Common
{
    public class CreateMultiPaymentRequest : MoipRequest
    {
        private CreateMultiPaymentRequest(string multiOrderId, object multiPaymentData)
            : base($"multiorders/{multiOrderId}/multipayments", Method.POST)
        {
            AddParameter("application/json", PaymentIntegrator.Moip.Utils.JsonHelper.Serialize(multiPaymentData), ParameterType.RequestBody);
        }

        public static CreateMultiPaymentRequest Create(string multiOrderId, IList<IOrder> orders, ICustomer customer, IPaymentOptions paymentOptions)
        {
            if (multiOrderId == null)
                throw new ArgumentNullException(nameof(multiOrderId));

            if (orders == null)
                throw new ArgumentNullException(nameof(orders));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (paymentOptions == null)
                throw new ArgumentNullException(nameof(paymentOptions));

            var multiPaymentData = CreateMultiPayment(orders, customer, paymentOptions);
            return new CreateMultiPaymentRequest(multiOrderId, multiPaymentData);
        }

        private static MoipPayment CreateMultiPayment(IList<IOrder> orders, ICustomer customer, IPaymentOptions paymentOptions)
        {
            var payment = new MoipPayment
            {
                InstallmentCount = paymentOptions.Installments,
                FundingInstrument = MoipPaymentMethods.CreatePaymentMethod(paymentOptions, customer),
                StatementDescriptor = MoipConfig.Current.UseMarketplaceName ?
                    StringHelper.Truncate(MaaSConfig.Current.ApplicationName, 13) : 
                    StringHelper.Truncate(orders.First().Supplier.AlternativeName, 13) ?? 
                    StringHelper.Truncate(orders.First().Supplier.Name, 13)
            };

            if (MoipConfig.Current.PaymentWarrantyEnabled)
            {
                payment.Escrow = new
                {
                    description = "Pagamento em garantia",
                };
            }

            return payment;
        }
    }

    public class CreateMultiPaymentResult : MoipResponse
    {
        private CreateMultiPaymentResult(IRestResponse restResponse, CreateMultiPaymentResultData data) : base(restResponse)
        {
            Data = data;
        }

        public static CreateMultiPaymentResult Create(IRestResponse restResponse)
        {
            var data = PaymentIntegrator.Moip.Utils.JsonHelper.Deserialize<CreateMultiPaymentResultData>(restResponse.Content);
            return new CreateMultiPaymentResult(restResponse, data);
        }

        public CreateMultiPaymentResultData Data { get; }

        protected override HttpStatusCode[] SuccessfulStatusCodes => new[] { HttpStatusCode.OK, HttpStatusCode.Created };

        public class CreateMultiPaymentResultData
        {
            public string Id { get; private set; }
            public string Status { get; private set; }
            public int InstallmentCount { get; private set; }
            public MultiPaymentItem[] Payments { get; private set; }
            public bool PaymentIsDenied => Payments.First().PaymentIsDenied;

            public class MultiPaymentItem
            {
                public string Status { get; private set; }
                public MultiPaymentItemAmount Amount { get; private set; }
                public bool PaymentIsDenied => !"AUTHORIZED,WAITING".Split(',').Contains(Status);
            }

            public class MultiPaymentItemAmount
            {
                public int Fees { get; set; }
                public int Refunds { get; set; }
                public int Liquid { get; set; }
                public string Currency { get; set; }
                public int Total { get; set; }
            }
        }
    }
}
