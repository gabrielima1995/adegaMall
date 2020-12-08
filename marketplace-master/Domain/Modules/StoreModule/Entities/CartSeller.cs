using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.StoreModule
{
    public class CartSeller : BaseEntity<Guid>
    {
        [Required]
        public Cart Cart { get; set; }

        //[ForeignKey("Cart")]
        //public Guid Cart_Id { get; set; }

        [Required]
        public Seller Seller { get; set; }

        //[ForeignKey("Seller")]
        //public string Seller_Id { get; set; }

        public decimal SellerShipping { get; set; }
        public int SellerDeliveryDays { get; set; }
        public decimal SellerDiscount { get; set; }

        public CartSeller()
        {
            Id = Guid.NewGuid();
        }

        public CartSeller(Cart cart, Seller seller, decimal sellerShipping)
            : this()
        {
            Cart = cart;
            Seller = seller;
            SellerShipping = sellerShipping;
        }
    }
}
