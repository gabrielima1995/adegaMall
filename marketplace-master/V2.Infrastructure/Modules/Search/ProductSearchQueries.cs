using Application.Config;
using Application.Modules.Products.Queries;
using Dapper;
using Domain.Entities;
using Domain.Entities.StoreModule;
using Domain.StoreModule.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace V2.Infrastructure.Modules.Search
{
    public class ProductSearchQueries : IProductSearchQueries
    {
        public ReadOnlyCollection<Product> ListAll()
        {
            var shippingPerStore = MaaSConfig.Current.Product.ShippingMode == ShippingMode.PerStore;
            var shippingPerProduct = MaaSConfig.Current.Product.ShippingMode == ShippingMode.PerProduct;
            var noShippingEnabled = shippingPerProduct && MaaSConfig.Current.Product.NoShipping.Enabled;
            var actualNoShippingEnabled = shippingPerProduct && MaaSConfig.Current.Product.ActualNoShipping.Enabled;
            var privateFleetShippingEnabled = MaaSConfig.Current.Product.PrivateFleetShipping.Enabled;
            var sedexShippingEnabled = shippingPerProduct && MaaSConfig.Current.Product.SedexShipping.Enabled;
            var pacShippingEnabled = shippingPerProduct && MaaSConfig.Current.Product.PACShipping.Enabled;
            var onSitePickupShippingEnabled = MaaSConfig.Current.Product.OnSitePickupShipping.Enabled;
            var requireTemplateId = MaaSConfig.Current.Product.Templates.Enabled && MaaSConfig.Current.Product.Templates.ForceSellerToUseTemplates;

            var subscriptionEnabled = MaaSConfig.Current.Subscription.IsRequired && !MaaSConfig.Current.Payment.PagarMe.UndergoingValidation;
            var integratorEnabled = MaaSConfig.Current.Payment.Enabled;

            var connStr = ConfigurationManager.ConnectionStrings["ScaffoldContext"].ConnectionString;

            using (var conn = new SqlConnection(connStr))
            {
                var query = conn.QueryMultiple(sql, new
                {
                    shippingPerStore,
                    shippingPerProduct,
                    noShippingEnabled,
                    privateFleetShippingEnabled,
                    sedexShippingEnabled,
                    pacShippingEnabled,
                    onSitePickupShippingEnabled,
                    subscriptionEnabled,
                    integratorEnabled,
                    actualNoShippingEnabled,
                    requireTemplateId
                });

                var products = query.Read<Product, Store, Seller, Address, Domain.Entities.SubscriptionModule.Subscription, ProductCategory, Product>(
                    (product, store, seller, addr, subscription, category) =>
                    {
                        product.Store = store;
                        product.Store.Seller = seller;
                        product.Store.Seller.Address = addr;
                        product.Store.Seller.Subscription = subscription;
                        product.Category = category;
                        return product;
                    },
                    "Id,SellerId,Id,Id,SellerId,Id").ToList();

                var grid = query.Read<RentalSettingsAux>();
                foreach (var rentalSettings in grid)
                {
                    var product = products.Find(x => x.Id == rentalSettings.Id);
                    if (product != null)
                    {
                        product.RentalSettings = new RentalSettings
                        {
                            MinRentalPeriod = rentalSettings.RentalSettings_MinRentalPeriod,
                            MaxRentalPeriod = rentalSettings.RentalSettings_MaxRentalPeriod,
                            MinRentalNotice = rentalSettings.RentalSettings_MinRentalNotice,
                            MinRentalNoticeUnit = rentalSettings.RentalSettings_MinRentalNoticeUnit,
                            AvailabilityWarning = rentalSettings.RentalSettings_AvailabilityWarning,
                            ChargeDoubleShipping = rentalSettings.RentalSettings_ChargeDoubleShipping,
                            RentalPeriodUnit = rentalSettings.RentalSettings_RentalPeriodUnit,
                            MinRentalMargin = rentalSettings.RentalSettings_MinRentalMargin,
                            RentalMarginUnit = rentalSettings.RentalSettings_RentalMarginUnit,
                            MinRentalAmount = rentalSettings.RentalSettings_MinRentalAmount
                        };
                    }
                }

                var images = query.Read<MyProductImage>();
                var privateFleetParams = query.Read<MyPrivateFleetParameter>();
                var variants = query.Read<ProductVariant, ProductVariantType, ProductVariantProduct, ProductVariantProduct>(
                    (v, t, rel) =>
                    {
                        v.Type = t;
                        rel.Variant = v;
                        return rel;
                    },
                    "Id,Id,ProductVariant_Id");

                var singleFreightParams = query.Read<MySingleFreightParameter>();

                foreach (var p in products)
                {
                    p.Images = images
                        .Where(x => x.Product_Id == p.Id)
                        .Select(x => (ProductImage)x)
                        .ToList();

                    p.PrivateFleetParams = p.PrivateFleetEnabled == false ? new List<PrivateFleetParameter>() : privateFleetParams
                        .Where(x => x.Product_Id == p.Id)
                        .Select(x => (PrivateFleetParameter)x)
                        .ToList();

                    p.Variants = variants
                        .Where(x => x.Product_Id == p.Id)
                        .Select(x => x.Variant)
                        .ToList();

                    p.Store.SingleFreightParams = p.Store.PrivateFleetEnabled == false ? new List<SingleFreightParameter>() : singleFreightParams
                        .Where(x => x.Store_SellerId == p.Store.SellerId)
                        .Select(x => (SingleFreightParameter)x)
                        .ToList();
                }

                products = products
                    .Where(product => !product.IsDisabled.GetValueOrDefault())
                    .Where(x => !x.IsBillingProduct.GetValueOrDefault())
                    .ToList();

                return products.AsReadOnly();
            }
        }

        private const string sql = @"
            SELECT
	            Product.*,
	            Store.*,
	            Seller.*,
	            [Address].*,
	            Subscription.*,
	            ProductCategory.*
            FROM Product
            LEFT JOIN Store ON Product.Store_SellerId = Store.SellerId
            LEFT JOIN Person AS Seller ON Store.SellerId = Seller.Id
            LEFT JOIN [Address] ON Seller.Address_Id = [Address].Id
            LEFT JOIN Subscription ON Seller.Id = Subscription.SellerId
            LEFT JOIN ProductCategory ON Product.Category_Id = ProductCategory.Id
            WHERE
	            Seller.[Enabled] = 1
                AND Product.Deleted = 0
	            AND (Product.CurrentStock > 0 OR NoShippingEnabled = 1)
                AND (@requireTemplateId = 0 OR Product.ProductTemplateId IS NOT NULL)
                AND (@subscriptionEnabled = 0 OR Subscription.Status = 1 /* trial */ OR Subscription.Status = 2 /* paid */)
                AND (@integratorEnabled = 0 OR Seller.SupplierGatewayId IS NOT NULL)
                AND (
                    (@noShippingEnabled = 0 OR Product.NoShippingEnabled = 1) OR
                    (@actualNoShippingEnabled = 0 OR Product.ActualNoShippingEnabled = 1) OR
                    (@onSitePickupShippingEnabled = 0 OR
                        (@shippingPerProduct = 1 AND Product.OnSitePickupEnabled = 1) OR
                        (@shippingPerStore = 1 AND Store.OnSitePickupEnabled = 1)) OR

                    (@sedexShippingEnabled = 0 OR
                        (@shippingPerProduct = 1 AND Product.SedexEnabled = 1) OR
                        (@shippingPerStore = 1 AND Store.SedexEnabled = 1)) OR

                    (@pacShippingEnabled = 0 OR
                        (@shippingPerProduct = 1 AND Product.PACEnabled = 1) OR
                        (@shippingPerStore = 1 AND Store.PacEnabled = 1)) OR

                    (@privateFleetShippingEnabled = 0 OR (
                        (@shippingPerProduct = 1 AND Product.PrivateFleetEnabled = 1) OR
                        (@shippingPerStore = 1 AND Store.PrivateFleetEnabled = 1))))

            SELECT Id,
               RentalSettings_MinRentalPeriod,
	           RentalSettings_MaxRentalPeriod,
	           RentalSettings_MinRentalNotice,
	           RentalSettings_MinRentalNoticeUnit,
	           RentalSettings_AvailabilityWarning,
	           RentalSettings_ChargeDoubleShipping,
               RentalSettings_RentalPeriodUnit,
               RentalSettings_MinRentalMargin,
               RentalSettings_RentalMarginUnit,
               RentalSettings_MinRentalAmount
            FROM dbo.Product

            SELECT *
            FROM ProductImage
            ORDER BY Product_Id;

            SELECT *
            FROM PrivateFleetParameter
            ORDER BY Product_Id;

            SELECT *
            FROM ProductVariant
            INNER JOIN ProductVariantType ON ProductVariant.[Type_Id] = ProductVariantType.Id
            INNER JOIN ProductVariantProduct ON ProductVariant.Id = ProductVariantProduct.ProductVariant_Id
            ORDER BY ProductVariantType.[Order], ProductVariantType.Name, ProductVariant.[Order], ProductVariant.Name

            SELECT *
            FROM SingleFreightParameter
            WHERE Store_SellerId IS NOT NULL
            ORDER BY Store_SellerId, DeliveryRange";

        private class ProductVariantProduct
        {
            public int ProductVariant_Id { get; set; }
            public Guid Product_Id { get; set; }
            public ProductVariant Variant { get; set; }
        }

        [NotMapped]
        private class MyProductImage : ProductImage
        {
            public Guid Product_Id { get; set; }
        }

        [NotMapped]
        private class MyPrivateFleetParameter : PrivateFleetParameter
        {
            public Guid Product_Id { get; set; }
        }

        [NotMapped]
        private class MySingleFreightParameter : SingleFreightParameter
        {
            public string Store_SellerId { get; set; }
        }

        private class RentalSettingsAux
        {
            public Guid Id { get; set; }
            public int? RentalSettings_MinRentalPeriod { get; set; }
            public int? RentalSettings_MaxRentalPeriod { get; set; }
            public int? RentalSettings_MinRentalNotice { get; set; }
            public RentalPeriodUnit? RentalSettings_MinRentalNoticeUnit { get; set; }
            public string RentalSettings_AvailabilityWarning { get; set; }
            public bool RentalSettings_ChargeDoubleShipping { get; set; }
            public RentalPeriodUnit? RentalSettings_RentalPeriodUnit { get; set; }
            public int? RentalSettings_MinRentalMargin { get; set; }
            public RentalPeriodUnit? RentalSettings_RentalMarginUnit { get; set; }
            public decimal? RentalSettings_MinRentalAmount { get; set; }
        }
    }
}
