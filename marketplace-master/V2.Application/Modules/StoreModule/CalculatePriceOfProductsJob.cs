using Application.Config;
using Domain.Interfaces.Repositories;
using Domain.Modules.StoreModule.Price;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace V2.Application.Modules.StoreModule
{
    public class CalculatePriceOfProductsJob
    {
        private readonly IProductRepository _productRepository;
        private readonly IRentalShiftQueries _rentalShiftQueries;

        public CalculatePriceOfProductsJob(
            IProductRepository productRepository,
            IRentalShiftQueries rentalShiftQueries)
        {
            _productRepository = productRepository;
            _rentalShiftQueries = rentalShiftQueries;
        }

        public void Run()
        {
            var products = _productRepository.ListByPrice();

            var shifts = MaaSConfig.Current.Product.BusinessModel.Rental.Enabled && MaaSConfig.Current.Product.BusinessModel.Rental.RentalPeriodMode == RentalPeriodMode.Shifts
                ? _rentalShiftQueries.ListRentalShifts().ToList()
                : new List<RentalShiftDto>();

            foreach (var product in products)
            {
                var productPrice = ProductPrice.ByPrice(product,
                    product.Price.GetValueOrDefault(), product.PriceOriginal, shifts);

                product.SetPrice(productPrice);

                _productRepository.Update(product, true);
            }
        }
    }
}