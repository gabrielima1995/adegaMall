using Domain.Entities.StoreModule;

namespace Domain.DTO
{
    public class ParcelInfo
    {
        public ParcelDeliveryMode DeliveryMode { get; set; }

        public decimal Height { get; set; }

        public decimal Length { get; set; }

        public decimal Weight { get; set; }

        public decimal Width { get; set; }

        public string ZipCodeFrom { get; set; }

        public string ZipCodeTo { get; set; }

        public ParcelInfo()
        {
        }

        public ParcelInfo(Product product, string zipCodeFrom, string zipCodeTo, ParcelDeliveryMode deliveryMode)
        {
            Height = product.Height ?? 0M;
            Length = product.Length ?? 0M;
            Weight = product.Weight ?? 0M;
            Width = product.Width ?? 0M;

            ZipCodeFrom = zipCodeFrom;
            ZipCodeTo = zipCodeTo;
            DeliveryMode = deliveryMode;
        }
    }

    public enum ParcelDeliveryMode
    {
        Normal = 0,
        Express = 1
    }
}
