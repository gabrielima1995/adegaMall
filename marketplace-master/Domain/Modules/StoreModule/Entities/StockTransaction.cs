using Domain.Events;
using Framework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.StoreModule
{
    public class StockTransaction : BaseEntity<Guid>
    {
        public Order Order { get; set; }

        public Guid? Product_Id { get; set; }

        [Required]
        [ForeignKey("Product_Id")]
        public Product Product { get; set; }

        public StockTransactionType Type { get; set; }

        public decimal Amount { get; set; }

        public int? Variant_Id { get; set; }

        [ForeignKey("Variant_Id")]
        public ProductVariant Variant { get; set; }

        [Column(TypeName = "DATETIME2")]
        public DateTime DateCreated { get; set; }

        public StockTransaction()
        {
            Id = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
        }

        [Obsolete("Use the new static 'Create' methods")]
        public StockTransaction(Order order, Product product, ProductVariant variant, decimal amount, StockTransactionType type) : this()
        {
            Order = order;
            Product = product;
            Variant = variant;
            Amount = amount;
            Type = type;
        }

        public static StockTransaction Create(Guid id, Product product, ProductVariant variant, decimal amount, StockTransactionType type)
        {
            Guard.Ensure.AreNotEqual(type, StockTransactionType.NewSale, nameof(type));
            Guard.Ensure.AreNotEqual(type, StockTransactionType.SaleCancelledOrRejected, nameof(type));

            var stockTransaction = new StockTransaction
            {
                Id = id,
                DateCreated = DateTime.UtcNow,
                Product = product,
                Variant = variant,
                Amount = amount,
                Type = type
            };

            stockTransaction.AddDomainEvent(new ProductMovedEvent(product, variant, amount, type));
            return stockTransaction;
        }

        public static StockTransaction Create(Guid id, Order order, Product product, ProductVariant variant, decimal amount, StockTransactionType type)
        {
            Guard.Ensure.AreNotEqual(type, StockTransactionType.ManualUpdate, nameof(type));
            Guard.Ensure.AreNotEqual(type, StockTransactionType.StockOpening, nameof(type));

            var stockTransaction = new StockTransaction
            {
                Id = id,
                DateCreated = DateTime.UtcNow,
                Order = order,
                Product = product,
                Variant = variant,
                Amount = amount,
                Type = type
            };

            stockTransaction.AddDomainEvent(new ProductMovedEvent(order, product, variant, amount, type));
            return stockTransaction;
        }
    }
}