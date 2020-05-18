using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Domain.DTO
{
    public class OrderItemListing
    {
        public Guid Id { get; set; }

        public string CustomId { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(MoneyJsonConverter))]
        public decimal UnitPrice { get; set; }

        public decimal Quantity { get; set; }

        [JsonConverter(typeof(MoneyJsonConverter))]
        public decimal ShippingPrice { get; set; }

        [JsonConverter(typeof(MoneyJsonConverter))]
        public decimal ItemTotal => (Quantity * UnitPrice) + ShippingPrice;
    }
}