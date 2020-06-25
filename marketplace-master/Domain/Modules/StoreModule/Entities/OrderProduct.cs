using Application.Config;
using Domain.Events.StoreModule;
using Domain.Modules.StoreModule.Entities;
using Domain.Services.StoreModule;
using Domain.StoreModule.Entities;
using Framework;
using PaymentIntegrator.Core.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Domain.Entities.StoreModule
{
    /// <summary>
    /// Produto de um pedido feito pelo comprador para o vendedor.
    /// </summary>
    public class OrderProduct : BaseEntity<Guid>, IPaymentItem
    {
        /// <summary>
        /// Produto do pedido.
        /// </summary>
        public Product Product { get; set; }

        [ForeignKey("Product")]
        public Guid? Product_Id { get; set; }

        /// <summary>
        /// Pedido relacionado ao produto.
        /// </summary>
        public Order Order { get; set; }

        [ForeignKey("Order")]
        public Guid Order_Id { get; set; }

        /// <summary>
        /// Titulo do produto do pedido.
        /// </summary>
        public string Item { get; set; }

        /// <summary>
        /// Imagem do produto do pedido.s
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Categoria do produto do pedido.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Quantidade do produto no pedido.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Valor bruto do produto no pedido.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Valor do frete para o produto do pedido.
        /// </summary>
        public decimal Shipping { get; set; }

        /// <summary>
        /// Link do produto digital
        /// </summary>
        public string NoShippingUrl { get; set; }

        /// <summary>
        /// Indica se o produto foi comprado por caracteristica.
        /// </summary>
        public bool ByVariant { get; set; }

        /// <summary>
        /// Variante do produto.
        /// </summary>
        public ProductVariant Variant { get; set; }

        [ForeignKey("Variant")]
        public int? Variant_Id { get; set; }

        /// <summary>
        /// Nome da caracteristica.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// Nome do tipo da caracteristica
        /// </summary>
        public string VariantTypeName { get; set; }

        /// <summary>
        /// Valor total bruto do produto no pedido.
        /// </summary>
        public decimal SubTotal => Quantity * Price;

        /// <summary>
        /// Valor total líquido do produto no pedido.
        /// </summary>
        public decimal Amount => SubTotal + Shipping;

        public string FullItemName => Item + (ByVariant ? $" - {VariantTypeName}: {VariantName}" : "");

        public int Multiplier { get; set; }

        public decimal MultiplierPrice { get; set; }

        public OrderProductRentalInfo RentalInfo { get; set; }

        public IList<OrderProductRentalsInfo> RentalsInfo { get; private set; } = new List<OrderProductRentalsInfo>();

        /// <summary>
        /// Tipo do produto.
        /// </summary>
        public ProductType ProductType { get; set; }

        /// <summary>
        /// Total de unidades para produtos do tipo Pacote de créditos.
        /// </summary>
        public int? PackageCredits { get; set; }

        #region Policies

        public CancelOrderPolicy CancelOrderPolicy { get; set; }
        public int CancelOrderPolicyHours { get; set; }
        public ChangePeriodRentalPolicy ChangePeriodRentalPolicy { get; set; }
        public int ChangePeriodRentalPolicyHours { get; set; }

        #endregion Policies

        public bool IsPackageProduct => ProductType == ProductType.Package;

        public int TotalPossibleRentals => Order.IsRental
            ? (IsPackageProduct
                ? PackageCredits.GetValueOrDefault(0) * (int)Quantity
                : 1)
            : 0;

        public int TotalRentals => Order.IsRental
            ? (IsPackageProduct
                ? RentalsInfo.Count
                : 1)
            : 0;

        [NotMapped]
        public ErrorDictionary Errors { get; }

        public bool PriceRequired => !(MaaSConfig.Current.Product.BusinessModel.Rental.EnablePriceMultiplier && (Multiplier * MultiplierPrice) > 0);

        public DateTime? MinRentalStartDate => RentalsInfo.Any()
            ? RentalsInfo.Min(i => i.StartDate)
            : RentalInfo.StartDate;

        public DateTime? MaxRentalStartDate => RentalsInfo.Any()
            ? RentalsInfo.Max(i => i.StartDate)
            : RentalInfo.StartDate;

        public DateTime? MaxRentalEndDate => RentalsInfo.Any()
            ? RentalsInfo.Max(i => i.EndDate)
            : RentalInfo.EndDate;

        public OrderProduct()
        {
            Id = Guid.NewGuid();
            Errors = new ErrorDictionary();
            RentalInfo = new OrderProductRentalInfo();
        }

        public OrderProduct(CartItem item, RentalService rentalService)
            : this()
        {
            Product_Id = item.Product.Id;
            Product = item.Product;
            Item = item.Product.Title;
            Image = item.Product.CoverImageUrl;
            Category = item.Product.Category?.Name;
            Quantity = item.Quantity;
            Price = item.PriceWithDiscount;
            Shipping = item.Shipping;

            ByVariant = item.Variant != null;
            Variant_Id = item.Variant?.Id;
            Variant = item.Variant;
            VariantName = item.Variant?.Name;
            VariantTypeName = item.Product.StockVariantType?.Name;

            if (Product.NoShippingEnabled)
                NoShippingUrl = Product.NoShippingUrl;

            var stockValidation = Product.CheckStock(item.Quantity, item.Variant?.Id);
            if (!stockValidation.OK) Errors.Merge(stockValidation.Errors);

            if (MaaSConfig.Current.Product.BusinessModel.Rental.Enabled && item.RentalInfo.RentalPeriodUnit.HasValue)
            {
                if (item.Product.ProductType == ProductType.Default || item.RentalInfo.ProductAvailability_Id.HasValue)
                {
                    var rentalValidation = rentalService.CheckAvailability(Product, item.Quantity,
                        item.RentalInfo.StartDate.Value, item.RentalInfo.EndDate.Value,
                        item.Cart.Buyer.PreferredTimeZone,
                        productAvailabilityId: item.RentalInfo.ProductAvailability_Id);

                    if (!rentalValidation.OK)
                        Errors.Merge(rentalValidation.Errors);
                }

                Rental(item);
            }

            Multiplier = item.Multiplier;
            MultiplierPrice = item.Product.MultiplierPrice.GetValueOrDefault();
            ProductType = item.Product.ProductType;
            PackageCredits = item.Product.PackageCredits;

            #region Policies

            CancelOrderPolicy = item.Product.CancelOrderPolicy;
            CancelOrderPolicyHours = MaaSConfig.Current.Product.BusinessModel.Rental.Buyer.Cancel.BeforeInHours;
            ChangePeriodRentalPolicy = item.Product.ChangePeriodRentalPolicy;
            ChangePeriodRentalPolicyHours = MaaSConfig.Current.Product.BusinessModel.Rental.Buyer.ChangePeriod.BeforeInHours;

            #endregion Policies
        }

        /// <summary>
        /// Locação de produto
        /// </summary>
        /// <returns></returns>
        public OrderProduct Rental(CartItem cartItem)
        {
            bool chargeDoubleShipping = false;

            switch (MaaSConfig.Current.Product.ShippingMode)
            {
                case ShippingMode.PerProduct:
                    chargeDoubleShipping = cartItem.Product.RentalSettings.ChargeDoubleShipping ?? false;
                    break;

                case ShippingMode.PerStore:
                    chargeDoubleShipping = cartItem.Product.Store.RentalSettings.ChargeDoubleShipping ?? false;
                    break;
            }

            RentalInfo = new OrderProductRentalInfo(cartItem.RentalInfo, chargeDoubleShipping);

            return this;
        }

        /// <summary>
        /// Produto entregue
        /// </summary>
        /// <returns></returns>
        public OrderProduct Delivered()
        {
            Product.Delivered(this);

            return this;
        }

        /// <summary>
        /// Produto devolvido
        /// </summary>
        /// <returns></returns>
        public OrderProduct Returned()
        {
            Product.Returned(this);

            if (Order.IsRental)
            {
                RentalInfo.SetAsReturned();
            }

            return this;
        }

        /// <summary>
        /// Obtem as informações de agendamento do produto no pedido.
        /// </summary>
        /// <param name="rentalInfoId">Id do agendamento quando o produto for do tipo Pacote de créditos.</param>
        /// <returns></returns>
        public OperationResult<IOrderProductRentalInfo> GetRentalInfo(Guid? rentalInfoId = null)
        {
            if (!Order.IsRental) return new OperationResult<IOrderProductRentalInfo>("", "Order is not rental.");

            if (!IsPackageProduct) return new OperationResult<IOrderProductRentalInfo>(RentalInfo);

            var rentalInfo = RentalsInfo.FirstOrDefault(i => rentalInfoId == null || i.Id == rentalInfoId);

            return rentalInfo == null
                ? new OperationResult<IOrderProductRentalInfo>("", "Rental info not found by id.")
                : new OperationResult<IOrderProductRentalInfo>(rentalInfo);
        }

        /// <summary>
        /// Altera o período de um agendamento do produto no pedido.
        /// </summary>
        /// <param name="rentalStartUtc">Data e horal inicial.</param>
        /// <param name="rentalEndUtc">Data e hora final.</param>
        /// <param name="rentalInfoId">Identificador do agendamento quando o produto for do tipo Pacote de créditos.</param>
        /// <param name="productAvailabilityId">Identificador da configuração de disponibilidade.</param>
        /// <returns></returns>
        public OperationResult ChangeRental(DateTime rentalStartUtc, DateTime rentalEndUtc, Guid? rentalInfoId, Guid? productAvailabilityId)
        {
            var getResult = GetRentalInfo(rentalInfoId);

            if (getResult.IsFailure)
                return getResult;

            var rentalInfo = getResult.Result;
            rentalInfo.ChangePeriod(rentalStartUtc, rentalEndUtc, productAvailabilityId);

            AddDomainEvent(new RentalInfoUpdated(Order_Id, Order.Seller.Id, this));

            return OperationResult.Success;
        }

        /// <summary>
        /// Cria novo período de agendamento do produto no pedido.
        /// </summary>
        /// <param name="rentalStartUtc">Data e horal inicial.</param>
        /// <param name="rentalEndUtc">Data e hora final.</param>
        /// <param name="productAvailabilityId">Identificador da configuração de disponibilidade.</param>
        /// <returns></returns>
        public OperationResult NewRental(DateTime rentalStartUtc, DateTime rentalEndUtc, Guid? productAvailabilityId)
        {
            RentalsInfo.Add(new OrderProductRentalsInfo(this, rentalStartUtc, rentalEndUtc, productAvailabilityId));

            AddDomainEvent(new RentalInfoCreated(Order_Id, Order.Seller.Id, this));

            return OperationResult.Success;
        }

        public int GetTotalCompletedRentals(DateTime untilUtc)
        {
            if (Order.IsRental)
            {
                if (IsPackageProduct)
                {
                    return RentalsInfo.Count(i => i.HasValue && i.StartDate <= untilUtc);
                }

                if (RentalInfo.HasValue && RentalInfo.StartDate <= untilUtc)
                    return 1;
            }

            return 0;
        }

        public decimal GetDiscount()
        {
            var discountPerc = Order.Discount / SubTotal;
            return Math.Round(Price * discountPerc, 2);
        }

        public decimal GetCreditPrice()
        {
            var discountAmount = GetDiscount();

            return (Price - discountAmount) / PackageCredits.GetValueOrDefault(0);
        }
    }
}