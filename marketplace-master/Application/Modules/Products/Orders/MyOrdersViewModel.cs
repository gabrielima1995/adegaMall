using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace Application.Modules.Products.Orders
{
    public class MyOrdersViewModel
    {
        public IList<Order> Orders { get; }

        public MyOrdersViewModel()
        {
        }

        public MyOrdersViewModel(IList<Order> orders)
            : this()
        {
            Orders = orders;
        }
    }
}