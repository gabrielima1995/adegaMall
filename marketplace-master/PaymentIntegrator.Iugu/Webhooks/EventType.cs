using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace PaymentIntegrator.Iugu.Webhooks
{
    [JsonConverter(typeof(EventTypeJsonConverter))]
    public enum EventType
    {
        [Description("all")]
        All = 0,
        [Description("invoice.created")]
        InvoiceCreated,
        [Description("invoice.status_changed")]
        InvoiceStatusChanged,
        [Description("invoice.refund")]
        InvoiceRefund,
        [Description("invoice.payment_failed")]
        InvoicePaymentFailed,
        [Description("invoice.dunning_action")]
        InvoiceDunningAction,
        [Description("invoice.due")]
        InvoiceDue,
        [Description("invoice.installment_released")]
        InvoiceInstallmentReleased,
        [Description("invoice.released")]
        InvoiceReleased,
        [Description("subscription.suspended")]
        SubscriptionSuspended,
        [Description("subscription.activated")]
        SubscriptionActivated,
        [Description("subscription.created")]
        SubscriptionCreated,
        [Description("subscription.renewed")]
        SubscriptionRenewed,
        [Description("subscription.expired")]
        SubscriptionExpired,
        [Description("subscription.changed")]
        SubscriptionChanged,
        [Description("referrals.verification")]
        ReferralsVerification,
        [Description("referrals.bank_verification")]
        ReferralsBankVerification,
        [Description("withdraw_request.created")]
        WithdrawRequestCreated,
        [Description("withdraw_request.status_changed")]
        WithdrawRequestStatusChanged
    }

    internal static class EventTypeHelper
    {
        public static string GetDescription(this EventType eventType)
        {
            var type = typeof(EventType);
            var memberInfo = type.GetMember(eventType.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            return ((DescriptionAttribute)attributes[0]).Description;
        }

        public static EventType ParseDescription(string description)
        {
            var enumType = typeof(EventType);
            var enumNames = Enum.GetNames(enumType);

            foreach (var enumName in enumNames)
            {
                var memberInfo = enumType.GetMember(enumName);
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                var enumDescription = ((DescriptionAttribute)attributes[0]).Description;

                if (enumDescription.Equals(description, StringComparison.OrdinalIgnoreCase))
                    return (EventType)Enum.Parse(enumType, enumName);
            }

            throw new ArgumentOutOfRangeException(nameof(description));
        }
    }

    public class EventTypeJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonValue = (string)reader.Value;
            return EventTypeHelper.ParseDescription(jsonValue);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((EventType)value).GetDescription());
        }
    }
}
