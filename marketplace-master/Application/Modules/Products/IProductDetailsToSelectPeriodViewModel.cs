using Domain.Entities.StoreModule;
using Domain.Modules.StoreModule.Price;
using Domain.StoreModule.Entities;

namespace Application.Modules.Products
{
    public interface IProductDetailsToSelectPeriodViewModel
    {
        Product Product { get; }

        bool IsFixedRental { get; }

        ProductPrice PriceModel { get; }

        RentalPeriodUnit? RentalPeriodUnit { get; }
    }
}