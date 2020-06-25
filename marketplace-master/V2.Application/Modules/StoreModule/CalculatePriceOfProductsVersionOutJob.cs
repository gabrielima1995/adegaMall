using System.Collections.Generic;
using System.Linq;
using Application.Config;
using Domain.Interfaces.Repositories;
using Domain.Modules.StoreModule.Price;
using Domain.StoreModule.Entities;
using Domain.StoreModule.Interfaces;

namespace V2.Application.Modules.StoreModule
{
    public class CalculatePriceOfProductsVersionOutJob
    {
        private readonly IProductRepository _productRepository;
        private readonly IRentalShiftQueries _rentalShiftQueries;

        public CalculatePriceOfProductsVersionOutJob(
            IProductRepository productRepository,
            IRentalShiftQueries rentalShiftQueries)
        {
            _productRepository = productRepository;
            _rentalShiftQueries = rentalShiftQueries;
        }

        public void RunIfNeeded()
        {
            if (!ShouldRun())
                return;

            var products = _productRepository.ListByPriceVersionOut();

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

        private bool ShouldRun() => MaaSConfig.Current.Product.Enabled && MaaSConfig.Current.Product.Price.Enabled;
    }
}