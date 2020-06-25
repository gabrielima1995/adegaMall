using Application.Config;
using System.Collections.Generic;

namespace Domain.Modules.BatchImport
{
    public class ProductMetadata : ImportExportMetadata
    {
        public const int MaxAllowedInstallmentsPerProduct = 12;

        public ProductMetadata(IEnumerable<Field> fields) : base(fields)
        {
        }

        public static ProductMetadata Create()
        {
            var mandatory = true;
            var config = MaaSConfig.Current;

            var fields = new List<Field>
            {
                new Field(nameof(ProductData.CustomId), "PRODUCT_CUSTOM_ID_LABEL", DataType.AlphanumericCode, Prefix("CUSTOM_ID")),
                new Field(nameof(ProductData.Title), "PRODUCT_TITLE_LABEL", DataType.NameOrTitle, Prefix("TITLE"), mandatory),
                new Field(nameof(ProductData.Description), "PRODUCT_DESCRIPTION_LABEL", DataType.GeneralText, Prefix("DESCRIPTION"), mandatory),
                new Field(nameof(ProductData.Specification), "PRODUCT_SPECIFICATIONS_LABEL", DataType.GeneralText, Prefix("SPECIFICATIONS"), mandatory),
                new Field(nameof(ProductData.Price), "PRODUCT_PRICE_LABEL", DataType.Price, Prefix("PRICE"), mandatory),
                new Field(nameof(ProductData.CurrentStock), "PRODUCT_STOCK_CURRENT_LABEL", DataType.Quantity, Prefix("CURRENT_STOCK"), mandatory),
                new Field(nameof(ProductData.DeleteWhenStockZero), "PRODUCT_DELETE_WHEN_STOCK_ZERO_LABEL", DataType.YesNo, Prefix("DELETE_WHEN_STOCK_ZERO"), mandatory),
                new Field(nameof(ProductData.CategoryName), "PRODUCT_CATEGORIES_SELECT", DataType.NameOrTitle, Prefix("CATEGORY"), mandatory),
                new Field(nameof(ProductData.SellerEmail), "BATCH_IMPORT_PRODUCT_SELLER_EMAIL_LABEL", DataType.Email, Prefix("SELLER_EMAIL"), mandatory)
            };

            if (config.Product.EnableInstallmentsPerProduct)
                fields.Add(new Field(nameof(ProductData.MaxInstallments), "PRODUCT_MAX_INSTALLMENTS_LABEL", DataType.Quantity, Prefix("MAX_INSTALLMENTS")));

            if (config.Product.PrivateFleetShipping.Enabled)
            {
                fields.Add(new Field(nameof(ProductData.Shipping_SupportsPrivateShipping), "PRODUCT_PRIVATE_FLEET_METHOD_LABEL", DataType.YesNo, Prefix("SUPPORTS_PRIVATE_FLEET"), mandatory));
                fields.Add(new Field(nameof(ProductData.Shipping_PrivateShipping_Range), "PRODUCT_DELIVERY_RANGE_LABEL", DataType.Quantity, Prefix("PRIVATE_FLEET_RANGE"), mandatory));
                fields.Add(new Field(nameof(ProductData.Shipping_PrivateShipping_Price), "PRODUCT_SHIPPING_LABEL", DataType.Price, Prefix("PRIVATE_FLEET_PRICE"), mandatory));
            }

            if (config.Product.SedexShipping.Enabled)
                fields.Add(new Field(nameof(ProductData.Shipping_SupportsSedexShipping), "PRODUCT_SEDEX_METHOD_LABEL", DataType.YesNo, Prefix("SUPPORTS_SEDEX")));

            if (config.Product.PACShipping.Enabled)
                fields.Add(new Field(nameof(ProductData.Shipping_SupportsPacShipping), "PRODUCT_PAC_METHOD_LABEL", DataType.YesNo, Prefix("SUPPORTS_PAC")));

            if (config.Product.OnSitePickupShipping.Enabled)
                fields.Add(new Field(nameof(ProductData.Shipping_SupportsOnSitePickup), "PRODUCT_ON_SITE_PICKUP_METHOD_LABEL", DataType.YesNo, Prefix("SUPPORTS_ON_SITE_PICKUP")));

            if (config.Product.ActualNoShipping.Enabled)
                fields.Add(new Field(nameof(ProductData.Shipping_NoShipping), "PRODUCT_ACTUAL_NO_SHIPPING_METHOD_LABEL", DataType.YesNo, Prefix("SUPPORTS_NO_SHIPPING")));

            if (config.Product.NoShipping.Enabled)
            {
                fields.Add(new Field(nameof(ProductData.Shipping_SupportsDownload), "PRODUCT_NO_SHIPPING_METHOD_LABEL", DataType.YesNo, Prefix("SUPPORTS_DOWNLOAD")));
                fields.Add(new Field(nameof(ProductData.Shipping_DownloadUrl), "PRODUCT_NO_SHIPPING_URL_LABEL", DataType.Url, Prefix("DOWNLOAD_URL")));
            }

            if (config.Product.HasPhysicalShipping)
            {
                fields.Add(new Field(nameof(ProductData.Shipping_PackageHeight), "PRODUCT_HEIGHT_LABEL", DataType.PackageSizeUnitInCm, Prefix("HEIGHT"), mandatory));
                fields.Add(new Field(nameof(ProductData.Shipping_PackageLength), "PRODUCT_LENGTH_LABEL", DataType.PackageSizeUnitInCm, Prefix("LENGTH"), mandatory));
                fields.Add(new Field(nameof(ProductData.Shipping_PackageWeight), "PRODUCT_WEIGHT_LABEL", DataType.PackageWeightInKg, Prefix("WEIGHT"), mandatory));
                fields.Add(new Field(nameof(ProductData.Shipping_PackageWidth), "PRODUCT_WIDTH_LABEL", DataType.PackageSizeUnitInCm, Prefix("WIDTH"), mandatory));
            }

            return new ProductMetadata(fields);
        }

        private static string Prefix(string label) => "BATCH_IMPORT_SAMPLE_PRODUCT_" + label;
    }
}
