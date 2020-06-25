using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Framework;
using System;
using System.Collections.Generic;

namespace Domain.StoreModule.Interfaces
{
    public interface IShippingAddressRepository
    {
        OperationResult<ShippingAddress> GetById(string shippingAddressId);
        ShippingAddress GetByAddressId(string addressId);
        OperationResult Delete(ShippingAddress entity);
    }
}
