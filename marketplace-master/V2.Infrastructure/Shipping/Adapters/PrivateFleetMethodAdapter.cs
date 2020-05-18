using Application.Config;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using V2.Infrastructure.Shipping.Adapters;

namespace Infrastructure.Shipping.Adapters
{
    /// <summary>
    /// Adapter para método de entrega frota própria/privada.
    /// </summary>
    public sealed class PrivateFleetMethodAdapter : ShippingOutAdapterBase
    {
        private readonly MaaSConfig config = MaaSConfig.Current;

        public PrivateFleetMethodAdapter() : base(ShippingMethod.PrivateFleet)
        {
        }

        public override DateTime? CalculateDeliveryDate(int deliveryDays)
        {
            throw new InvalidOperationException("Shipping method 'PrivateFleetMethodAdapter' does not have shipping days.");
        }

        public override OperationResult<CorreiosShippingCollection> GetShippingForProduct(ProductWithQty product, Address sellerAddress, Address shippingAddress, bool chargeDoubleShipping)
        {
            var config = MaaSConfig.Current;

            if (config.Product.ShippingMode == ShippingMode.PerProduct)
            {
                if (product.Product.PrivateFleetEnabled)
                {
                    if (shippingAddress == null)
                        return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("NO_SHIPPING_ADDRESS"));

                    if (!shippingAddress.HasLocation)
                        return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("SHIPPING_ADDRESS_NO_LOCATION"));

                    if (sellerAddress == null || !sellerAddress.HasLocation)
                        return new ErrorDictionary("",
                            string.Format(CultureInfo.CurrentCulture.GetStringLocalized("SELLER_ADDRESS_NO_LOCATION"),
                            product.Product.Store?.Seller?.PublicProfileName ?? CultureInfo.CurrentCulture.GetStringLocalized("SELLER_TITLE")));

                    var distance = sellerAddress.Location.Distance(shippingAddress.Location);

                    var shippingParam = product.Product.PrivateFleetParams
                        .Where(i => i.DeliveryRange > (distance.Value / 1000))
                        .OrderBy(i => i.DeliveryRange)
                        .FirstOrDefault();

                    if (shippingParam != null)
                    {
                        var listCorreiosShippingDto = new List<CorreiosShippingDto>()
                        {
                            new CorreiosShippingDto(
                                shippingParam.Id,
                                "Processamento com sucesso",
                                "0",
                                shippingParam.Shipping * product.Quantity,
                                resultType: Domain.StoreModule.Entities.CorreiosResultType.Success,
                                deliveryDays: 1)
                        };

                        return new CorreiosShippingCollection(listCorreiosShippingDto, chargeDoubleShipping);
                    }


                    return new ErrorDictionary(product.Product.Id.ToString(), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_SELLER_NO_SHIPPING"));
                }

                return new ErrorDictionary(product.Product.Id.ToString(), CultureInfo.CurrentCulture.GetStringLocalized("PRODUCT_PRIVATE_FLEET_NOT_ENABLED"));
            }

            else return CorreiosShippingCollection.Empty();
        }

        public override OperationResult<CorreiosShippingCollection> GetShippingForSeller(IEnumerable<ProductWithQty> products, Store store, Address shippingAddress)
        {
            if (store?.Seller?.Address == null)
                throw new ArgumentNullException(nameof(store.Seller.Address));

            if (config.Product.ShippingMode == ShippingMode.PerStore)
            {
                if (store.PrivateFleetEnabled)
                {
                    var chargeDoubleShipping =
                        config.Product.BusinessModel.Rental.Enabled &&
                        (store.RentalSettings.ChargeDoubleShipping ?? false);

                    #region Validação de endereço

                    if (shippingAddress == null)
                        return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("NO_SHIPPING_ADDRESS"));

                    if (!shippingAddress.HasLocation)
                        return new ErrorDictionary("", CultureInfo.CurrentCulture.GetStringLocalized("SHIPPING_ADDRESS_NO_LOCATION"));

                    if (!store.Seller.Address.HasLocation)
                        return new ErrorDictionary("",
                            string.Format(CultureInfo.CurrentCulture.GetStringLocalized("SELLER_ADDRESS_NO_LOCATION"),
                            store.Seller.PublicProfileName ?? CultureInfo.CurrentCulture.GetStringLocalized("SELLER_TITLE")));

                    #endregion Validação de endereço

                    #region Obtem parametro de entrega

                    var distance = store.Seller.Address.Location.Distance(shippingAddress.Location);

                    var shippingParam = store.SingleFreightParams
                        .Where(i => i.DeliveryRange > (distance.Value / 1000))
                        .OrderBy(i => i.DeliveryRange)
                        .FirstOrDefault();

                    #endregion Obtem parametro de entrega

                    if (shippingParam != null)
                    {
                        #region Calcula número de entregas

                        var deliveryNumber = 1;

                        if (config.Product.BusinessModel.Rental.Enabled)
                        {
                            // calcula numero de entregas pela data de locação
                            deliveryNumber = products.GroupBy(i => i.RentalStart).Count();
                        }

                        #endregion Número de entregas

                        // Valor da entrega * número de entregas * Frete dobrado ou não
                        var correiosShippingDto = new List<CorreiosShippingDto>(){ new CorreiosShippingDto(
                            shippingParam.Id,
                            "Processamento com sucesso",
                            "0",
                            shippingParam.Shipping * deliveryNumber,
                            resultType: Domain.StoreModule.Entities.CorreiosResultType.Success,
                            deliveryDays: 1) };

                        return new CorreiosShippingCollection(correiosShippingDto, chargeDoubleShipping);
                    }

                    return new ErrorDictionary(store.SellerId, CultureInfo.CurrentCulture.GetStringLocalized("STORE_NO_SHIPPING"));
                }

                return new ErrorDictionary(store.SellerId, CultureInfo.CurrentCulture.GetStringLocalized("STORE_PRIVATE_FLEET_NOT_ENABLED"));
            }

            return CorreiosShippingCollection.Empty();
        }

        public override OperationResult<Uri> GetTrackingLink(string trackingCode)
        {
            if (string.IsNullOrEmpty(trackingCode))
                return new OperationResult<Uri>("", "NO_TRACKING_CODE_SUPPLIED");

            return new OperationResult<Uri>(new Uri($"https://www.google.com/{trackingCode}"));
        }
    }
}