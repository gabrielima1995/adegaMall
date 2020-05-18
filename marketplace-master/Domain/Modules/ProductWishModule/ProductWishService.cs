using Domain.Entities;
using Domain.Entities.StoreModule;
using Framework;
using System;

namespace Domain.Modules.ProductWishModule
{
    public class ProductWishService : IProductWishService
    {
        private readonly IProductWishRepository _repository;

        public ProductWishService(IProductWishRepository repository)
        {
            _repository = repository;
        }

        public OperationResult Add(Buyer buyer, Product product)
        {
            var productWish = new ProductWish(buyer, product);

            _repository.Add(productWish);

            return OperationResult.Success;
        }

        public OperationResult Remove(string buyerId, Guid productId)
        {
            var productWish = _repository.Get(buyerId, productId);

            _repository.Delete(productWish);

            return OperationResult.Success;
        }
    }
}