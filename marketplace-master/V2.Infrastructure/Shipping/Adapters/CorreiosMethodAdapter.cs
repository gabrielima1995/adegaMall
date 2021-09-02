using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Domain.StoreModule.Entities;
using Framework;
using Infra.Crosscutting.Globalization;
using Nager.Date;
using Nager.Date.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace V2.Infrastructure.Shipping.Adapters
{
    public sealed class SedexMethodAdapter : CorreiosMethodAdapter
    {
        public SedexMethodAdapter() : base(ShippingMethod.PostalExpress) { }
    }

    public sealed class PacMethodAdapter : CorreiosMethodAdapter
    {
        public PacMethodAdapter() : base(ShippingMethod.PostalRegular) { }
    }

    public class CorreiosMethodAdapter : ShippingOutAdapterBase
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        public bool UseSedex { get; private set; }

        public CorreiosMethodAdapter(ShippingMethod shippingMethod) : base(shippingMethod)
        {
            switch (shippingMethod)
            {
                case ShippingMethod.PostalExpress:
                    UseSedex = true;
                    break;

                case ShippingMethod.PostalRegular:
                    UseSedex = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Shipping method must be either PostalExpress or PostalRegular", nameof(shippingMethod));
            }
        }

        public override OperationResult<CorreiosShippingCollection> GetShippingForProduct(ProductWithQty product, Address sellerAddress, Address shippingAddress, bool chargeDoubleShipping)
        {
            if (UseSedex ? product.Product.Store.SedexEnabled : product.Product.Store.PacEnabled)
            {
                var products = new[] { product };
                var shipping = GetShipping(products, product.Product.Store?.Seller?.PublicProfileName, sellerAddress, shippingAddress, chargeDoubleShipping).Result;

                shipping = ApplyDefaultShippingForCommunicationErrors(shipping, products, null);
                return shipping;
            }

            return CorreiosShippingCollection.Empty();
        }

        public override OperationResult<CorreiosShippingCollection> GetShippingForSeller(IEnumerable<ProductWithQty> products, decimal discount, Store store, Address shippingAddress)
        {
            if (config.Product.ShippingMode == ShippingMode.PerStore)
            {
                if (UseSedex ? store.SedexEnabled : store.PacEnabled)
                {
                    var deliveryBatches = new List<IEnumerable<ProductWithQty>>();
                    if (config.Product.BusinessModel.Rental.Enabled)
                    {
                        deliveryBatches = products
                            .GroupBy(i => i.RentalStart, i => i, (key, value) => value)
                            .ToList();
                    }
                    else deliveryBatches.Add(products);

                    var allErrors = new ErrorDictionary();
                    var shippingCollection = CorreiosShippingCollection.Empty();

                    foreach (var batch in deliveryBatches)
                    {
                        var batchShipping = GetShipping(batch, store.Seller.PublicProfileName, store.Seller.Address, shippingAddress, store.RentalSettings.ChargeDoubleShipping);

                        if (batchShipping.IsFailure) allErrors.Merge(batchShipping.Errors);
                        else shippingCollection = shippingCollection.Merge(batchShipping.Result);
                    }

                    if (allErrors.HasErrors)
                        return allErrors;

                    shippingCollection = ApplyDefaultShippingForCommunicationErrors(shippingCollection, products, store);
                    return shippingCollection;
                }
            }

            return new ErrorDictionary(store.SellerId, CultureInfo.CurrentCulture.GetStringLocalized(UseSedex ? "STORE_SEDEX_NOT_ENABLED" : "STORE_PAC_NOT_ENABLED"));
        }

        private CorreiosShippingCollection ApplyDefaultShippingForCommunicationErrors(CorreiosShippingCollection collection, IEnumerable<ProductWithQty> products, Store store)
        {
            var newList = new List<CorreiosShippingDto>();
            if (collection == null) collection = CorreiosShippingCollection.Empty();
            foreach (var item in collection)
            {
                if (item.ResultType == CorreiosResultType.CommunicationError)
                {
                    var product = products.Single(p => p.Product.Id == item.ItemId);
                    var defaultShipping = (new[] { product.Product.DefaultShipping, product.Product.Store?.DefaultShipping, store?.DefaultShipping }).FirstOrDefault(d => d != null && d.IsConfigured);

                    if (defaultShipping != null)
                    {
                        newList.Add(item.ConvertToDefaultShipping(
                            defaultShipping.Amount * product.Quantity,
                            defaultShipping.DeliveryDays));

                        continue;
                    }
                }

                newList.Add(item);
            }

            return new CorreiosShippingCollection(newList, collection.ChargeDoubleShipping);
        }

        private OperationResult<CorreiosShippingCollection> GetShipping(IEnumerable<ProductWithQty> items, string sellerName, Address origin, Address destination, bool? StorechargeDoubleShipping)
        {
            if (destination == null)
                return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("NO_SHIPPING_ADDRESS"));

            if (!destination.HasLocation)
                return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("SHIPPING_ADDRESS_NO_LOCATION"));

            if (origin == null || !origin.HasLocation)
            {
                return new ErrorDictionary("",
                    string.Format(CultureInfo.CurrentCulture.GetStringLocalized("SELLER_ADDRESS_NO_LOCATION"),
                    sellerName ?? CultureInfo.CurrentCulture.GetStringLocalized("SELLER_TITLE")));
            }

            if (items.Any(x => x.Product.Weight == null || x.Product.Length == null || x.Product.Width == null || x.Product.Height == null))
            {
                return new ErrorDictionary("",
                    string.Format(CultureInfo.CurrentCulture.GetStringLocalized("SELLER_PRODUCT_DIMENSIONS_ABSENT"),
                    sellerName ?? CultureInfo.CurrentCulture.GetStringLocalized("SELLER_TITLE")));
            }

            var correios = new CorreiosShippingHelper();
            var itemsCorreios = GetItemsCorreios(items);

            var chargeDoubleShipping = config.Product.BusinessModel.Rental.Enabled &&
            (StorechargeDoubleShipping ?? false);

            var correiosResult = UseSedex
                ? correios.CalculateSedexShipping(itemsCorreios, origin, destination)
                : correios.CalculatePACShipping(itemsCorreios, origin, destination);

            var correiosDtoCollection = new List<CorreiosShippingDto>();

            for (int i = 0; i < items.Count(); i++)
            {
                correiosDtoCollection.Add(new CorreiosShippingDto(items.ElementAt(i).Product.Id,
                    correiosResult.Result.Message,
                    correiosResult.Result.CodeResult,
                    config.Product.ShippingMode == ShippingMode.PerProduct || i == 0 ? correiosResult.Result.ShippingPrice : 0m,
                    correiosResult.Result.Type,
                    correiosResult.Result.DeliveryDays));
            }
            return new CorreiosShippingCollection(correiosDtoCollection, chargeDoubleShipping);
        }

        private ReadOnlyCollection<ItemFrete> GetItemsCorreios(IEnumerable<ProductWithQty> products)
        {
            var listaCorreio = new List<ItemFrete>(products.Sum(x => x.Quantity));

            foreach (var item in products)
            {
                for (var i = 0; i < item.Quantity; i++)
                {
                    listaCorreio.Add(new ItemFrete(
                        (double)item.Product.Weight,
                        item.Product.BasePrice.GetValueOrDefault(0),
                        (double)item.Product.Length,
                        (double)item.Product.Width,
                        (double)item.Product.Height));
                }
            }

            return listaCorreio.AsReadOnly();
        }

        public override OperationResult<Uri> GetTrackingLink(string trackingCode)
        {
            if (string.IsNullOrEmpty(trackingCode))
                return new OperationResult<Uri>("", GlobalizationHelper.GetString("NO_TRACKING_CODE_SUPPLIED"));

            return new OperationResult<Uri>(new Uri($"https://www.linkcorreios.com.br/{trackingCode}"));
        }

        public override DateTime? CalculateDeliveryDate(int deliveryDays)
        {
            if (deliveryDays == 0)
                return null;
            var date = DateTime.Today;
            Enum.TryParse(Regex.Replace(CultureInfo.CurrentCulture.GetCurrentLang().ToUpper(), ".*-", ""), out CountryCode countryCode);
            date = date.AddDays(1);

            if (DateSystem.IsPublicHoliday(date, countryCode) || date.IsWeekend(countryCode))
                date = SumDayForShippingOrReceive(date, countryCode);

            date = date.AddDays(deliveryDays);
            if (DateSystem.IsPublicHoliday(date, countryCode) || date.IsWeekend(countryCode))
                date = SumDayForShippingOrReceive(date, countryCode);

            date = date.AddDays(1);
            if (DateSystem.IsPublicHoliday(date, countryCode) || date.IsWeekend(countryCode))
                date = SumDayForShippingOrReceive(date, countryCode);
            return date;
        }

        private DateTime SumDayForShippingOrReceive(DateTime date, CountryCode countryCode)
        {
            do
            {
                date = date.AddDays(1);
            } while (DateSystem.IsPublicHoliday(date, countryCode) || date.IsWeekend(countryCode));
            return date;
        }
    }
}