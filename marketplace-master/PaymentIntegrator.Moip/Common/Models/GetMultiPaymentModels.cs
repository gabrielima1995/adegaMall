using Newtonsoft.Json;
using PaymentIntegrator.Core.PaymentMethods;
using PaymentIntegrator.Moip.Utils;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace PaymentIntegrator.Moip.Common.Models
{
    public class PaymentId : IEquatable<PaymentId>
    {
        private static readonly Regex oldFormat = new Regex(@"MPY\-[\da-zA-Z]+");
        private static readonly Regex newFormat = new Regex(@"ORD\-[\da-zA-Z]+");

        private PaymentId(string value)
        {
            Value = value;
        }

        public string Value { get; }
        public string Request { get; set; }

        public static PaymentId Create(string value)
        {
            if (!oldFormat.IsMatch(value) && !newFormat.IsMatch(value))
                throw new Exception($"Value '{value}' is not valid for a MultiPaymentId. A valid value looks like 'MPY-000000000000' or 'ORD-000000000000'");

            return new PaymentId(value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PaymentId);
        }

        public bool Equals(PaymentId other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<string>.Default.GetHashCode(Value);
        }

        public override string ToString() => Value;

        public static implicit operator string(PaymentId multiPaymentId) => multiPaymentId.Value;

        public static explicit operator PaymentId(string value) => Create(value);
        public bool IsMultiPayment => this.Value.StartsWith("MPY");
    }

    public class GetPaymentRequest : MoipRequest
    {
        public GetPaymentRequest(PaymentId paymentId)
            : base(paymentId.Request, Method.GET)
        {
        }

        public static GetPaymentRequest Create(PaymentId paymentId)
        {
            var endpoint = paymentId.IsMultiPayment ? "multipayments" : "orders";
            paymentId.Request = $"{endpoint}/{paymentId}";
            return new GetPaymentRequest(paymentId);
        }
    }

    public class GetMultiPaymentResult : MoipResponse
    {
        private GetMultiPaymentResult(IRestResponse restResponse, GetMultiPaymentResultData data) : base(restResponse)
        {
            Data = data;
        }

        public static GetMultiPaymentResult Create(IRestResponse restResponse)
        {
            var data = PaymentIntegrator.Moip.Utils.JsonHelper.Deserialize<GetMultiPaymentResultData>(restResponse.Content);

            if (data.IsBoleto)
            {
                var dynamicData = JsonConvert.DeserializeAnonymousType(restResponse.Content, new
                {
                    _links = new { checkout = new { payBoleto = new { printHref = "" } } }
                });

                data.BoletoUrl = dynamicData._links.checkout.payBoleto.printHref;
            }

            return new GetMultiPaymentResult(restResponse, data);
        }

        public GetMultiPaymentResultData Data { get; private set; }
        protected override HttpStatusCode[] SuccessfulStatusCodes => new[] { HttpStatusCode.OK };

        public class GetMultiPaymentResultData
        {
            public string Id { get; private set; }
            public string Status { get; private set; }
            public int InstallmentCount { get; private set; }
            public AmountData Amount { get; private set; }
            public PaymentData[] Payments { get; private set; }

            public bool IsBoleto => Payments.First().FundingInstrument.IsBoleto;
            public string BoletoUrl { get; internal set; }
            public string BoletoBarCode => Payments.First().FundingInstrument.Boleto?.LineCode;
            public DateTime? BoletoExpirationDate => Payments.First().FundingInstrument.Boleto?.ExpirationDate.ToEndOfDay();

            public bool IsPaymentAuthorized(PaymentMethod method) => Status == "AUTHORIZED" || (Status == "WAITING" && method == PaymentMethod.Boleto);

            public class AmountData
            {
                public int Total { get; set; }
                public string Currency { get; set; }
            }

            public class PaymentAmountData
            {
                public int Total { get; private set; }
                public int Fees { get; private set; }
                public string Currency { get; private set; }
            }

            public class PaymentData
            {
                public PaymentAmountData Amount { get; private set; }
                public int InstallmentCount { get; private set; }
                public FundingInstrumentData FundingInstrument { get; private set; }
                public ReceiverData[] Receivers { get; private set; }
                public CancellationDetailsData CancellationDetails { get; private set; }
                public OrderPayment _Links { get; private set; }
            }

            public class OrderPayment
            {
                public OrderId Order { get; private set; }
            }

            public class OrderId
            {
                public string Title { get; private set; }
            }

            public class ReceiverData
            {
                public string Type { get; private set; }
                public ReceiverAmountData Amount { get; private set; }
                public bool IsPrimary => Type == "PRIMARY";
            }

            public class ReceiverAmountData
            {
                public int Total { get; private set; }
                public int Refunds { get; private set; }
            }

            public class FundingInstrumentData
            {
                public string Method { get; private set; }
                public FundingInstrumentBoletoData Boleto { get; private set; }
                public bool IsBoleto => Method == "BOLETO";
            }

            public class FundingInstrumentBoletoData
            {
                public string LineCode { get; private set; }
                public DateTime ExpirationDate { get; private set; }
            }

            public class CancellationDetailsData
            {
                public string Code { get; private set; }
                public string Description { get; private set; }
                public string CancelledBy { get; private set; }
                public override string ToString() => $"Código {Code} - {Description}";
            }
        }
    }


}
