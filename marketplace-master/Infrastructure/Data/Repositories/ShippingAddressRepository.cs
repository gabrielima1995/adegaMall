using Domain.Entities.StoreModule;
using Domain.StoreModule.Interfaces;
using Framework;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class ShippingAddressRepository : EfCrudRepository, IShippingAddressRepository
    {
        public ShippingAddressRepository(ScaffoldContext context) : base(context)
        {
        }

        public OperationResult Delete(ShippingAddress entity) 
        {
           return Delete<ShippingAddress>(entity, true);
        }

        public OperationResult<ShippingAddress> GetById(string shippingAddressId)
        {
            return GetById<ShippingAddress>(shippingAddressId);
        }

        public ShippingAddress GetByAddressId(string addressId)
        {
            return Context.ShippingAddress.Where(x => x.Address.Id.ToString() == addressId).FirstOrDefault();
        }
    }
}
