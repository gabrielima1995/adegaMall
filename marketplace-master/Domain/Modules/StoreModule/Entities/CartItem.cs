using Domain.Modules.StoreModule.Entities;
using Domain.Modules.StoreModule.Price;
using Domain.StoreModule.Entities;
using System;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Item do carrinho de compras.
    /// </summary>
    public class CartItem : BaseEntity<Guid>
    {
        /// <summary>
        /// Carrinho de compra do item.
        /// </summary>
        public Cart Cart { get; set; }

        /// <summary>
        /// Produto relacionado.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Data em que o produto foi adicionado ao carrinho.
        /// </summary>
        public DateTime AddedAt { get; set; }

        /// <summary>
        /// Quantidade do produto.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Preço unitário do item (product.price).
        /// </summary>
        public decimal Price =>
            Product.BasePrice.GetValueOrDefault(0) *
            (RentalInfo.RentalPeriodValue != null && !ProductPriceSpecs.AllowsCalculateBasePrice(Product) && !Product.IsPackageType ? RentalInfo.RentalPeriodValue.Value : 1) +
            Product.GetAdditionalPrice(Multiplier).GetValueOrDefault();

        /// <summary>
        /// Valor do desconto do produto.
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// Preço unitário com desconto.
        /// </summary>
        public decimal PriceWithDiscount => Price - Discount;

        /// <summary>
        /// Preço unitário do item (product.price).
        /// </summary>
        public decimal MultiplierPrice => Product.MultiplierPrice.GetValueOrDefault();

        /// <summary>
        /// Valor do frete para o item.
        /// </summary>
        public decimal Shipping { get; set; }

        public int DeliveryDays { get; set; }

        /// <summary>
        /// Valor do desconto do item.
        /// </summary>
        public decimal DiscountTotal => Quantity * Discount;

        /// <summary>
        /// Sub total do item (quantidade * preço ).
        /// </summary>
        public decimal SubTotal => Quantity * PriceWithDiscount;

        /// <summary>
        /// Multiplicador adicional de produto
        /// </summary>
        public int Multiplier { get; set; }

        /// <summary>
        /// Indica se o produto for comprado por característica.
        /// </summary>
        public bool ByVariant { get; set; }

        /// <summary>
        /// Característica do produto.
        /// </summary>
        public ProductVariant Variant { get; set; }

        /// <summary>
        /// Valor total do item (quantidade * preço + frete).
        /// </summary>
        public decimal Amount => SubTotal + Shipping - DiscountTotal;

        public string FullProductTitle
        {
            get
            {
                try
                {
                    return Product.Title + (ByVariant ? $" - {Product.StockVariantType.Name}: {Variant.Name}" : "");
                }
                catch
                {
                    return Product.Title;
                }
            }
        }

        public CartItemRentalInfo RentalInfo { get; set; }

        public CartItem()
        {
            Id = Guid.NewGuid();
            AddedAt = DateTime.UtcNow;
            RentalInfo = CartItemRentalInfo.Empty();
        }

        public CartItem(Cart cart, Product product, int quantity, decimal shipping) : this()
        {
            Cart = cart;
            Quantity = quantity;
            Product = product;
            Shipping = shipping;
        }

        public CartItem Rental(DateTime startDateTime, DateTime endDateTime, RentalPeriodUnit periodUnit, ProductAvailability productAvailability)
        {
            RentalInfo = new CartItemRentalInfo(startDateTime, endDateTime, periodUnit, productAvailability);

            if (productAvailability != null && productAvailability.HasDiscount)
            {
                var discountPerc = productAvailability.DiscountPercentage;

                Discount = Math.Round(Price * discountPerc / 100M, 2, MidpointRounding.ToEven);
            }

            return this;
        }

        public CartItem Rental(int fixedPeriod, RentalPeriodUnit periodUnit)
        {
            RentalInfo = new CartItemRentalInfo(fixedPeriod, periodUnit);
            return this;
        }
    }
}