using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.StoreModule
{
    public class StockProductVariant : BaseEntity<int>
    {
        public int? Variant_Id { get; protected set; }

        [Required]
        [ForeignKey("Variant_Id")]
        public ProductVariant Variant { get; protected set; }

        public int CurrentStock { get; protected set; }

        [NotMapped]
        public int UpdatedStock { get; protected set; }

        public bool HasStock => CurrentStock > 0;

        public static StockProductVariant New(ProductVariant variant, int currentStock, int? updatedStock = null)
        {
            return new StockProductVariant
            {
                Variant_Id = variant?.Id,
                Variant = variant,
                CurrentStock = currentStock,
                UpdatedStock = updatedStock ?? currentStock
            };
        }

        public void NewStock(int currentStock)
        {
            CurrentStock = currentStock;
        }
    }
}