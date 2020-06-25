using Newtonsoft.Json;
using System;
using System.Globalization;

namespace PaymentIntegrator.Iugu.Common
{
    public class CurrencyJsonConverter : JsonConverter
    {
        private readonly CultureInfo _ptBrCulture = CultureInfo.CreateSpecificCulture("pt-BR");

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonValue = (string)reader.Value;
            if (string.IsNullOrEmpty(jsonValue)) return null;
            return Decimal.Parse(jsonValue, NumberStyles.Currency, _ptBrCulture);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteValue("");

            var str = ((decimal)value).ToString("{0:C2}", _ptBrCulture);
            writer.WriteValue(str);
        }
    }
}
