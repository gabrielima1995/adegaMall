using Domain.StoreModule.Entities;
using System.Linq;

namespace V2.Infrastructure.Shipping
{
    public class CorreioShippingResult
    {
        public CorreioShippingResult(string message, string codeResult, decimal shippingPrice, int deliveryDays)
        {
            Message = message;
            CodeResult = codeResult;
            ShippingPrice = shippingPrice;
            Type = GetResultType(codeResult);
            DeliveryDays = deliveryDays;
        }

        public string Message { get; }
        public string CodeResult { get; }
        public decimal ShippingPrice { get; }
        public int DeliveryDays { get; }

        public CorreiosResultType Type { get; }

        public CorreiosResultType GetResultType(string codeResult)
        {
            if (codeResult != null)
            {
                if (codeResult.Equals("0")) return CorreiosResultType.Success;
                if ("010,011".Split(',').Any(s => s.Equals(codeResult))) return CorreiosResultType.Warning;
                if (codeResult.Equals("comms")) return CorreiosResultType.CommunicationError;
            }
            return CorreiosResultType.Error;
        }
    }
}
