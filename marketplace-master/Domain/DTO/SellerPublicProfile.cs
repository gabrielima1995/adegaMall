using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Modules.Portfolio;
using System.Collections.Generic;

namespace Domain.DTO
{
    public class SellerPublicProfile
    {
        public Seller Seller { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public Portfolio Portfolio { get; set; }
    }
}
