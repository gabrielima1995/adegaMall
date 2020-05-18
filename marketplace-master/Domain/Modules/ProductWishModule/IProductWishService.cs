using Domain.Entities;
using Domain.Entities.StoreModule;
using Framework;
using System;

namespace Domain.Modules.ProductWishModule
{
    public interface IProductWishService
    {
        OperationResult Add(Buyer buyer, Product product);

        OperationResult Remove(string buyerId, Guid productId);
    }
}