namespace Domain.Modules.BatchImport
{
    public class ProductData
    {
        public string CustomId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public decimal? Price { get; set; }
        public int CurrentStock { get; set; }
        public bool DeleteWhenStockZero { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SellerId { get; set; }
        public string SellerEmail { get; set; }
        public int MaxInstallments { get; set; }
        public bool Shipping_SupportsPrivateShipping { get; set; }
        public int Shipping_PrivateShipping_Range { get; set; }
        public decimal Shipping_PrivateShipping_Price { get; set; }
        public bool Shipping_SupportsSedexShipping { get; set; }
        public bool Shipping_SupportsPacShipping { get; set; }
        public bool Shipping_SupportsOnSitePickup { get; set; }
        public bool Shipping_NoShipping { get; set; }
        public bool Shipping_SupportsDownload { get; set; }
        public string Shipping_DownloadUrl { get; set; }
        public decimal Shipping_PackageHeight { get; set; }
        public decimal Shipping_PackageLength { get; set; }
        public decimal Shipping_PackageWeight { get; set; }
        public decimal Shipping_PackageWidth { get; set; }

        //public ReadOnlyCollection<CustomFieldValueDto> CustomFields { get; set; } = new List<CustomFieldValueDto>().AsReadOnly();

        /* TODO:
         * Support cover image
         * Support additional images
         * Support variants
         * Support private fleet parameters
         * Support rental
         * Support custom fields
         */
    }
}
