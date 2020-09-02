using Application.Config;
using Domain.DTO;
using Domain.StoreModule.Entities;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Domain.Modules.StoreModule.Coupons;

namespace Domain.Entities.StoreModule
{
    public class Cart : BaseEntity<Guid>
    {
        public Buyer Buyer { get; set; }
        public Address ShippingAddress { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public IList<CartItem> Items { get; set; }
        public IList<CartSeller> Sellers { get; set; }
        public ShippingMode ShippingMode { get; set; }
        public decimal Shipping { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public Coupon Coupon { get; set; }
        public decimal Discount { get; set; }

        /// <summary>
        /// Indica se o carrinho foi transformado em pedido.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Informações ou instruções adicionais do pedido.
        /// </summary>
        public string ExtraInfo { get; set; }

        public int TotalItems => Items?.Count ?? 0;

        public bool IsEmpty => TotalItems == 0;

        public decimal SubTotal => Items?.Sum(x => x.SubTotal) ?? 0M;

        /// <summary>
        /// Indica a soma total de itens + frete do carrinho.
        /// </summary>
        public decimal Amount => SubTotal + Shipping - Discount;

        /// <summary>
        /// ID de sessão do usuário (AspNet SessionID).
        /// </summary>
        [NotMapped]
        public string SessionId { get; set; }

        /// <summary>
        /// CartService fornecia um método EnsureStockAndShipping que já era chamado pelo CartService.Get
        /// mas acabava sendo chamado por outros códigos no fluxo. Esta propriedade armazena o resultado
        /// que ocorre no Get e evita chamadas adicionais desnecessárias, já que há comunicação com o banco de dados.
        /// </summary>
        [NotMapped]
        public OperationResult<CorreiosShippingCollection> StockAndShippingValidation { get; set; }

        [NotMapped]
        public List<ShippingMethod> AvailableShippingsMethods { get; set; }

        public Cart()
        {
            Id = Guid.NewGuid();
            ShippingMethod = ShippingMethod.PrivateFleet;
            Items = new List<CartItem>();
            Sellers = new List<CartSeller>();
        }

        public CartItem GetItemByProduct(Product product, DateTime? rentalStartUtc = null, DateTime? rentalEndUtc = null, int? variant = null)
        {
            if (rentalStartUtc.HasValue && rentalEndUtc.HasValue)
                return Items.FirstOrDefault(i => i.Product.Id == product.Id && i.RentalInfo.StartDate == rentalStartUtc && i.RentalInfo.EndDate == rentalEndUtc);

            if (variant.HasValue)
                return Items.FirstOrDefault(i => i.Product.Id == product.Id && i.Variant.Id == variant);

            return Items.FirstOrDefault(i => i.Product.Id == product.Id);
        }

        public bool CheckRentalOverlap(Product product, DateTime? rentalStart, DateTime? rentalEnd)
        {
            if (!rentalStart.HasValue || !rentalEnd.HasValue)
                return false;

            var repeatingProducts = Items.Where(x => x.Product.Id == product.Id && x.RentalInfo.StartDate.HasValue && x.RentalInfo.EndDate.HasValue);

            foreach (var prod in repeatingProducts)
            {
                if (rentalStart > prod.RentalInfo.StartDate && rentalStart < prod.RentalInfo.EndDate)
                    return true;

                if (rentalEnd > prod.RentalInfo.StartDate && rentalEnd < prod.RentalInfo.EndDate)
                    return true;
            }

            return false;
        }

        public List<ShippingMethod> GetAvailableShippingMethods()
        {
            // Retorna somente os métodos de entrega que estão A) configurados e B)
            // têm algum produto no carrinho que aceita o método. Assim o usuário
            // fica selecionando métodos que não se adequam à compra atual.

            var list = new List<ShippingMethod>();

            foreach (var method in MaaSConfig.Current.Product.GetShippingMethodsEnabled())
            {
                if (MaaSConfig.Current.Product.ShippingMode == ShippingMode.PerProduct)
                {
                    foreach (var p in Items.Select(x => x.Product))
                    {
                        switch (method)
                        {
                            case ShippingMethod.ActualNoShipping:
                                if (p.ActualNoShippingEnabled && !list.Contains(ShippingMethod.ActualNoShipping))
                                    list.Add(ShippingMethod.ActualNoShipping);
                                break;

                            case ShippingMethod.NoShipping:
                                if (p.NoShippingEnabled && !list.Contains(ShippingMethod.NoShipping))
                                    list.Add(ShippingMethod.NoShipping);
                                break;

                            case ShippingMethod.PrivateFleet:
                                if (p.PrivateFleetEnabled && !list.Contains(ShippingMethod.PrivateFleet))
                                    list.Add(ShippingMethod.PrivateFleet);
                                break;

                            case ShippingMethod.OnSitePickup:
                                if (p.OnSitePickupEnabled && !list.Contains(ShippingMethod.OnSitePickup))
                                    list.Add(ShippingMethod.OnSitePickup);
                                break;

                            case ShippingMethod.PostalExpress:
                                if (p.SedexEnabled && !list.Contains(ShippingMethod.PostalExpress))
                                    list.Add(ShippingMethod.PostalExpress);
                                break;

                            case ShippingMethod.PostalRegular:
                                if (p.PACEnabled && !list.Contains(ShippingMethod.PostalRegular))
                                    list.Add(ShippingMethod.PostalRegular);
                                break;

                            case ShippingMethod.AsapLog:
                                if (!list.Contains(ShippingMethod.AsapLog))
                                    list.Add(ShippingMethod.AsapLog);
                                break;

                            case ShippingMethod.TotalExpress:
                                if (!list.Contains(ShippingMethod.TotalExpress))
                                    list.Add(ShippingMethod.TotalExpress);
                                break;
                        }
                    }
                }
                else
                {
                    foreach (var s in Items.Select(x => x.Product.Store))
                    {
                        switch (method)
                        {
                            case ShippingMethod.PrivateFleet:
                                if (s.PrivateFleetEnabled && !list.Contains(ShippingMethod.PrivateFleet))
                                    list.Add(ShippingMethod.PrivateFleet);
                                break;

                            case ShippingMethod.OnSitePickup:
                                if (s.OnSitePickupEnabled && !list.Contains(ShippingMethod.OnSitePickup))
                                    list.Add(ShippingMethod.OnSitePickup);
                                break;

                            case ShippingMethod.PostalExpress:
                                if (s.SedexEnabled && !list.Contains(ShippingMethod.PostalExpress))
                                    list.Add(ShippingMethod.PostalExpress);
                                break;

                            case ShippingMethod.PostalRegular:
                                if (s.PacEnabled && !list.Contains(ShippingMethod.PostalRegular))
                                    list.Add(ShippingMethod.PostalRegular);
                                break;

                            case ShippingMethod.AsapLog:
                                if (s.AsapLogEnabled && !list.Contains(ShippingMethod.AsapLog))
                                    list.Add(ShippingMethod.AsapLog);
                                break;

                            case ShippingMethod.TotalExpress:
                                if (s.TotalExpressEnabled && !list.Contains(ShippingMethod.TotalExpress))
                                    list.Add(ShippingMethod.TotalExpress);
                                break;
                        }
                    }
                }
            }

            //Coloca Sedex como primeiro item da lista
            if (MaaSConfig.Current.Product.PrioritizeExpressShipping && list.Contains(ShippingMethod.PostalExpress))
            {
                list.Remove(ShippingMethod.PostalExpress);
                list.Insert(0, ShippingMethod.PostalExpress);
            }

            return list;
        }

        public bool IsRentalWithDifferentsDates()
        {
            return MaaSConfig.Current.Product.BusinessModel.Rental.Enabled && Items
                .GroupBy(
                    i => i.Product.Store_SellerId,
                    i => i,
                    (key, values) => new
                    {
                        seller = key,
                        deliveryNumber = values
                            .GroupBy(j => j.RentalInfo.Hash)
                            .Count()
                    })
                .All(i => i.deliveryNumber > 1);
        }

        public OperationResult IsValidForCheckout()
        {
            var config = MaaSConfig.Current;
            var errors = new ErrorDictionary();

            if (Amount < config.Product.Order.MinOrderTotal)
            {
                var minAmountMsg = GlobalizationHelper.FormatString("ORDER_MIN_AMOUNT_NOT_MET", config.DefaultCurrency, config.Product.Order.MinOrderTotal);
                errors.Add("", minAmountMsg);
            }

            if (config.Product.Order.MinOrderBySeller > 0M)
            {
                var itemsBySeller = Items.GroupBy(x => x.Product.Store.SellerId);

                foreach (var group in itemsBySeller)
                {
                    var sum = group.Sum(x => x.SubTotal);

                    if (sum < config.Product.Order.MinOrderBySeller)
                    {
                        var msg = GlobalizationHelper.FormatString("ORDER_MIN_BY_SELLER_NOT_MET",
                            group.First().Product.Store.Seller.PublicProfileName,
                            config.DefaultCurrency,
                            config.Product.Order.MinOrderBySeller,
                            config.Product.Order.MinOrderBySeller - sum);
                        errors.Add("", msg);
                    }
                }
            }

            if (config.Product.BusinessModel.Rental.Enabled)
            {
                var itemsByProduct = Items.GroupBy(item => item.Product.Id);

                foreach (var groupedItems in itemsByProduct)
                {
                    var refProduct = groupedItems.First().Product;
                    var sum = groupedItems.Sum(item => item.SubTotal);
                    var min = refProduct.RentalSettings.MinRentalAmount;

                    if (sum < min)
                        errors.Add(refProduct.Id.ToString(), GlobalizationHelper.GetString("PRODUCT_MIN_RENTAL_AMOUNT_NOT_REACHED"));
                }

                if (HasOverlapingRentals())
                    errors.Add("", GlobalizationHelper.GetString("CART_HAS_CONFLICTING_RENTAL_PERIODS"));
            }
            if (StockAndShippingValidation.Errors.HasErrors && StockAndShippingValidation.Errors != null)
            {
                foreach (var item in StockAndShippingValidation.Errors)
                {
                    if (item.Key == "INVALID_COUPON")
                    {
                        errors.Add(item.Key, item.Value);
                    }
                    else
                    {
                        errors.Add(item.Key, GlobalizationHelper.GetString("CART_PRODUCT_SHIPPING_ERROR"));
                    }
                }
            }
            return new OperationResult(errors);
        }

        private bool HasOverlapingRentals()
        {
            var onlySameProduct = !MaaSConfig.Current.Product.BusinessModel.Rental.EnforceNoOverlapingInCart;

            for (int left = 0; left < Items.Count; left++)
            {
                for (int right = 0; right < Items.Count; right++)
                {
                    if (left == right)
                        continue;

                    if (onlySameProduct && Items[left].Product.Id != Items[right].Product.Id)
                        continue;

                    var infoEndDate = Items[right].RentalInfo.EndDate;
                    if (infoEndDate != null &&
                        (Items[left].RentalInfo.StartDate >= Items[right].RentalInfo.StartDate &&
                         Items[left].RentalInfo.StartDate <= infoEndDate.Value.AddMinutes(-1)))
                        return true;

                    var rentalInfoEndDate = Items[left].RentalInfo.EndDate;
                    if (rentalInfoEndDate != null &&
                        (rentalInfoEndDate.Value.AddMinutes(-1) >= Items[right].RentalInfo.StartDate &&
                         rentalInfoEndDate <= Items[right].RentalInfo.EndDate))
                        return true;
                }
            }

            return false;
        }
    }
}