using Domain.Entities;
using Domain.Entities.StoreModule;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Modules.ProductWishModule
{
    public class ProductWish : BaseEntity<Guid>
    {
        public string Buyer_Id { get; set; }

        public Guid Product_Id { get; set; }

        [ForeignKey("Buyer_Id")]
        public Buyer Buyer { get; set; }

        [ForeignKey("Product_Id")]
        public Product Product { get; set; }

        protected ProductWish()
        {
        }

        public ProductWish(Buyer buyer, Product product)
        {
            Id = Guid.NewGuid();

            Buyer_Id = buyer.Id;
            Buyer = buyer;

            Product_Id = product.Id;
            Product = product;
        }
    }
}