using Domain.Entities.StoreModule;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.DTO
{
    public class OrderListing
    {
        public Guid Id { get; set; }

        public Guid? OrderGroupId { get; set; }

        public DateTime CreatedAt { get; set; }

        public ShippingMethod ShippingMethod { get; set; }

        [JsonConverter(typeof(MoneyJsonConverter))]
        public decimal Shipping { get; set; }

        [JsonConverter(typeof(MoneyJsonConverter))]
        public decimal Discount { get; set; }

        [JsonConverter(typeof(MoneyJsonConverter))]
        public decimal ItemsTotal => Items.Sum(i => i.ItemTotal);

        [JsonConverter(typeof(MoneyJsonConverter))]
        public decimal OrderTotal => ItemsTotal + Shipping - Discount;

        public OrderStatus Status { get; set; }

        public OrderPaymentListing Payment { get; set; }

        public OrderBuyerListing Buyer { get; set; }

        public OrderSellerListing Seller { get; set; }

        public AddressListing SellerAddress { get; set; }

        public AddressListing ShippingAddress { get; set; }

        public IEnumerable<OrderHistoryListing> History { get; set; }

        public IEnumerable<OrderItemListing> Items { get; set; }

        public OrderListing()
        {
            History = new List<OrderHistoryListing>();
            Items = new List<OrderItemListing>();
        }
    }
}