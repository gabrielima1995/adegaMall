using Domain.StoreModule.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domain.DTO
{
    public class CorreiosShippingDto
    {
        public CorreiosShippingDto(Guid itemId, string message, string codeResult, decimal shippingPrice, CorreiosResultType resultType, int deliveryDays)
        {
            ItemId = itemId;
            Message = message;
            CodeResult = codeResult;
            ShippingPrice = shippingPrice;
            ResultType = resultType;
            DeliveryDays = deliveryDays;
        }

        public Guid ItemId { get; }
        public string Message { get; }
        public string CodeResult { get; }
        public decimal ShippingPrice { get; } 
        public int DeliveryDays { get; } 
        public CorreiosResultType ResultType { get; }
        public bool IsDefaultShipping { get; private set; }

        public CorreiosShippingDto ConvertToDefaultShipping(decimal price, int deliveryDays)
        {
            if (price <= 0)
                throw new ArgumentException("O frete padrão não pode ser gratuito ou negativo.");
            if (deliveryDays <= 0)
                throw new ArgumentException("O frete padrão não pode ter prazo de entrega zero.");
            if (ResultType != CorreiosResultType.CommunicationError)
                throw new InvalidOperationException($"Não é possível converter um {nameof(CorreiosShippingDto)} para frete padrão se não houver um erro de comunicação com os Correios.");

            return new CorreiosShippingDto(ItemId, Message, CodeResult, price, CorreiosResultType.CommunicationError, deliveryDays)
            {
                IsDefaultShipping = true
            };
        }
    }

    public class CorreiosShippingCollection : Collection<CorreiosShippingDto>
    {
        public CorreiosShippingCollection(IList<CorreiosShippingDto> list, bool chargeDoubleShipping) : base(list)
        {
            ChargeDoubleShipping = chargeDoubleShipping;
        }

        public static CorreiosShippingCollection Empty() => new CorreiosShippingCollection(new List<CorreiosShippingDto>(), false);

        public bool ChargeDoubleShipping  { get; } 

        public decimal TotalShippingPrice => Items.Sum(item => item.ShippingPrice) * (ChargeDoubleShipping ? 2 : 1);

        public int TotalDeliveryDays => Items.Count != 0 ? Items.Max(item => item.DeliveryDays) : 0;

        public CorreiosShippingCollection Merge(CorreiosShippingCollection other)
        {
            var allItems = Items.ToList();
            allItems.AddRange(other.Items);
            return new CorreiosShippingCollection(allItems, ChargeDoubleShipping);
        }
    }
}
