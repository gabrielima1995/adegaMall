using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Framework;
using Framework.TotalExpress;
using Framework.TotalExpress.ConsultaFrete;
using Infra.Crosscutting.Globalization;
using Infrastructure;
using Nager.Date;
using Nager.Date.Extensions;
using Unity;

namespace V2.Infrastructure.Shipping.Adapters
{
    public class TotalExpressMethodAdapter : ShippingOutAdapterBase
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        public TotalExpressMethodAdapter()
            : base(ShippingMethod.TotalExpress)
        {
        }

        public override OperationResult<CorreiosShippingCollection> GetShippingForProduct(ProductWithQty product, Address sellerAddress, Address shippingAddress, bool chargeDoubleShipping)
        {
            if (product.Product.Store.TotalExpressEnabled)
            {
                var products = new[] { product };

                return GetShipping(products, product.Product.Store, product.Product.Store?.Seller?.PublicProfileName, sellerAddress, shippingAddress, chargeDoubleShipping);
            }

            return CorreiosShippingCollection.Empty();
        }

        public override OperationResult<CorreiosShippingCollection> GetShippingForSeller(IEnumerable<ProductWithQty> products, Store store, Address shippingAddress)
        {
            if (config.Product.ShippingMode == ShippingMode.PerStore)
            {
                if (store.TotalExpressEnabled)
                {
                    var deliveryBatches = new List<IEnumerable<ProductWithQty>>();

                    if (config.Product.BusinessModel.Rental.Enabled)
                    {
                        deliveryBatches = products
                            .GroupBy(i => i.RentalStart, i => i, (key, value) => value)
                            .ToList();
                    }
                    else
                    {
                        deliveryBatches.Add(products);
                    }

                    var allErrors = new ErrorDictionary();
                    var shippingCollection = CorreiosShippingCollection.Empty();

                    foreach (var batch in deliveryBatches)
                    {
                        var batchShipping = GetShipping(batch, store, store.Seller.PublicProfileName, store.Seller.Address, shippingAddress, store.RentalSettings.ChargeDoubleShipping);

                        if (batchShipping.IsFailure) allErrors.Merge(batchShipping.Errors);
                        else shippingCollection = shippingCollection.Merge(batchShipping.Result);
                    }

                    if (allErrors.HasErrors)
                        return allErrors;

                    //shippingCollection = ApplyDefaultShippingForCommunicationErrors(shippingCollection, products, store);
                    return shippingCollection;
                }
            }

            return CorreiosShippingCollection.Empty();
        }

        private OperationResult<CorreiosShippingCollection> GetShipping(IEnumerable<ProductWithQty> items, Store store, string sellerName, Address origin, Address destination, bool? StorechargeDoubleShipping)
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

            var chargeDoubleShipping = config.Product.BusinessModel.Rental.Enabled && (StorechargeDoubleShipping ?? false);

            var itensFrete = GetItemsFrete(items);

            var pacote = new PacoteFrete(itensFrete);

            var credentials = new TotalExpressCredentials
            {
                Usuario = store.TotalExpressUser,
                Senha = store.TotalExpressPassword
            };

            if (!credentials.IsValid()) return new ErrorDictionary("", "Usuário e senha TotalExpress inválidos");

            var request = new ConsultaFreteRequest
            {
                Cep = destination.ZipCode,
                Peso = Math.Round((decimal)pacote.Peso, 1, MidpointRounding.ToEven),
                Valor = Math.Round(pacote.Valor, 2, MidpointRounding.ToEven),
                Altura = (int)Math.Ceiling(pacote.Altura),
                Profundidade = (int)Math.Ceiling(pacote.Comprimento),
                Largura = (int)Math.Ceiling(pacote.Largura)
            };

            var provider = UnityIocContainer.Instance.Resolve<ITotalExpressConsultaFreteProvider>();

            var response = provider.ConsultaFreteAsync(credentials, request);

            if (response.IsFailure)
            {
                var errorMessage = $"TotalExpress: {string.Join(". ", response.Errors.Select(i => i.Message))}";

                // todo: totalexpress notificar problema
                return new ErrorDictionary("", errorMessage);
            }

            var result = response.Result;

            var correiosDtoCollection = new List<CorreiosShippingDto>();

            for (int i = 0; i < items.Count(); i++)
            {
                correiosDtoCollection.Add(new CorreiosShippingDto(
                    items.ElementAt(i).Product.Id,
                    "Processamento com sucesso",
                    "0",
                    config.Product.ShippingMode == ShippingMode.PerProduct || i == 0 ? result.Preco : 0m,
                    resultType: Domain.StoreModule.Entities.CorreiosResultType.Success,
                    deliveryDays: result.Prazo));
            }

            return new CorreiosShippingCollection(correiosDtoCollection, chargeDoubleShipping);
        }

        private List<ItemFrete> GetItemsFrete(IEnumerable<ProductWithQty> products)
        {
            var items = new List<ItemFrete>(products.Sum(x => x.Quantity));

            foreach (var item in products)
            {
                for (var i = 0; i < item.Quantity; i++)
                {
                    items.Add(new ItemFrete(
                        (double)item.Product.Weight,
                        item.Product.BasePrice.GetValueOrDefault(0),
                        (double)item.Product.Length,
                        (double)item.Product.Width,
                        (double)item.Product.Height));
                }
            }

            return items;
        }

        //private CorreiosShippingCollection ApplyDefaultShippingForCommunicationErrors(CorreiosShippingCollection collection, IEnumerable<ProductWithQty> products, Store store)
        //{
        //    var newList = new List<CorreiosShippingDto>();

        //    if (collection == null) collection = CorreiosShippingCollection.Empty();

        //    foreach (var item in collection)
        //    {
        //        if (item.ResultType == CorreiosResultType.CommunicationError)
        //        {
        //            var product = products.Single(p => p.Product.Id == item.ItemId);
        //            var defaultShipping = (new[] { product.Product.DefaultShipping, product.Product.Store?.DefaultShipping, store?.DefaultShipping }).FirstOrDefault(d => d != null && d.IsConfigured);

        //            if (defaultShipping != null)
        //            {
        //                newList.Add(item.ConvertToDefaultShipping(
        //                    defaultShipping.Amount * (config.Product.ShippingMode == ShippingMode.PerProduct ? product.Quantity : 1),
        //                    defaultShipping.DeliveryDays));

        //                continue;
        //            }
        //        }

        //        newList.Add(item);
        //    }

        //    return new CorreiosShippingCollection(newList, collection.ChargeDoubleShipping);
        //}

        public override OperationResult<Uri> GetTrackingLink(string trackingCode)
        {
            if (string.IsNullOrEmpty(trackingCode))
                return new OperationResult<Uri>("", GlobalizationHelper.GetString("NO_TRACKING_CODE_SUPPLIED"));

            return new OperationResult<Uri>(new Uri($"https://www.linkcorreios.com.br/{trackingCode}")); //todo: asap tracking
        }

        public override DateTime? CalculateDeliveryDate(int deliveryDays)
        {
            if (deliveryDays == 0) return null;

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
