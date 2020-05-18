using Domain.Entities.StoreModule;
using Domain.Interfaces;

namespace Domain.DTO
{
    public class OrderDetails
    {
        public Order Order { get; set; }
        public IShippingMethodAdapter ShippingMethodAdapter { get; set; }

        public OrderDetails(Order order, IShippingMethodAdapter shippingMethodAdapter)
        {
            Order = order;
            ShippingMethodAdapter = shippingMethodAdapter;
        }
    }
}
