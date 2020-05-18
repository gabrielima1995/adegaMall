using Application.Config.Common;
using Domain.StoreModule.Entities;

namespace Domain.Config.Modules.Product
{
    public class RentalFixedPeriodForAllProductsConfig
    {
        public bool Enabled { get; set; } = false;
        public RentalPeriodUnit? FixedPeriodType { get; set; } = RentalPeriodUnit.Hour;
        public int? MinFixedPeriod { get; set; } = 1;
        public int? MaxFixedPeriod { get; set; } = 1;
    }
}