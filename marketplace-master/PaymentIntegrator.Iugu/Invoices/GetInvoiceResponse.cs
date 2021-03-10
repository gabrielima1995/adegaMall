using Newtonsoft.Json;
using PaymentIntegrator.Core.Payments;
using PaymentIntegrator.Iugu.Common;
using System;
using System.Collections.ObjectModel;

namespace PaymentIntegrator.Iugu.Invoices
{
    public class GetInvoiceResponse : ITransaction
    {
        [JsonProperty("id")]
        public string TransactionId { get; set; }

        public string Status { get; set; }

        public string RefuseReason { get; set; }

        public string Message { get; set; }

        [JsonProperty("total")]
        [JsonConverter(typeof(CurrencyJsonConverter))]
        public decimal Amount { get; set; }

        public decimal RefundedAmount { get; set; }

        [JsonProperty("comission")]
        [JsonConverter(typeof(CurrencyJsonConverter))]
        public decimal StartupFee { get; set; }

        public decimal RefundedStartupFee { get; set; }

        public string StartupSplitId { get; set; }

        [JsonProperty("taxes_paid")]
        [JsonConverter(typeof(CurrencyJsonConverter))]
        public decimal GatewayFee { get; set; }

        public bool Refundable { get; set; }

        public decimal RefundedGatewayFee { get; set; }

        public bool IsAntifraudRefuse() => false;

        [JsonProperty("notification_url")]
        public string PostbackUrl { get; set; }

        public string BoletoUrl => $"https://faturas.iugu.com/{Guid.Parse(TransactionId)}";

        public string BoletoBarCode { get; set; }

        public DateTime? BoletoExpirationDate { get; set; }

        public bool IsPaid() => "paid".Equals(Status, StringComparison.OrdinalIgnoreCase);

        public bool IsPendingPayment() => false;

        public bool IsPaymentCancelledOrRejected() => false;

        [JsonConverter(typeof(InstallmentsConverter))]
        public int Installments { get; set; }

        public ReadOnlyCollection<MultiPaymentItem> MultiPayments { get; }

        public bool IsMultiPayment { get; }

        public void SetRefundedValues(decimal refundedAmount, decimal refundedStartupFee, decimal refundedGatewayFee)
        {
            throw new NotImplementedException();
        }

        public void SetValuesForMultiPayment(string id, decimal amount, decimal startupFee)
        {
            throw new InvalidOperationException("GetInvoiceResponse class 'SetValuesForMultiPayment' does not have multipayments.");
        }
    }

    internal class InstallmentsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (existingValue == null)
                return 1;

            if (existingValue is int)
                return (int)existingValue;

            int installments;
            if (int.TryParse(existingValue.ToString(), out installments))
                return installments;

            throw new Exception($"Cannot parse the value '{existingValue}'; expected a valid integer.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value);
        }
    }
}
