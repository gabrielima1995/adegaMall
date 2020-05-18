using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace V2.Application.Modules.ProductWishModule
{
    public class MyWishesViewModel
    {
        public List<Product> Items { get; set; } = new List<Product>();
        public string NextPageUrl { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }


    }
}
