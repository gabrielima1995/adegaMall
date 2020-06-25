namespace Domain.Config.Sections
{
    public class NearbyProductsConfig
    {
        public bool Enabled { get; set; }
        public int TotalItems { get; set; } = 8;
        public double RangeInMeters { get; set; } = 5000D;
    }
}