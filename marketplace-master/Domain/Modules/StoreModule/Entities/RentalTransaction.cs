using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.StoreModule
{
    public class RentalTransaction : BaseEntity<Guid>
    {
        public Order Order { get; private set; }

        public OrderProduct OrderProduct { get; private set; }

        [Required]
        public Product Product { get; private set; }

        public RentalTransactionType Type { get; private set; }

        [Column(TypeName = "DATETIME2")]
        public DateTime DateCreated { get; private set; }

        private RentalTransaction(OrderProduct orderProduct, Product product, RentalTransactionType type)
        {
            Order = orderProduct.Order;
            OrderProduct = orderProduct;
            Product = product;
            Type = type;
            Id = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
        }

        public static RentalTransaction Create(OrderProduct orderProduct, Product product, RentalTransactionType type)
        {
            return new RentalTransaction(orderProduct, product, type);
        }
    }
}