using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace Application.Modules.Products.Orders
{
    public class MySalesViewModel
    {
        public IList<Order> Orders { get; }

        public MySalesViewModel()
        {

        }

        public MySalesViewModel(IList<Order> orders)
            : this()
        {
            Orders = orders;
        }
    }
}