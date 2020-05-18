using Domain.Entities.StoreModule;
using Domain.Interfaces;
using Infrastructure.Shipping.Adapters;
using System;
using System.Web.Caching;
using V2.Infrastructure.Shipping;
using V2.Infrastructure.Shipping.Adapters;

namespace Infrastructure.Shipping
{
    public class ShippingService : IShippingService
    {
        private readonly ICache cache;

		public ShippingService(ICache cache)
        {
            this.cache = cache;
        }

        public IShippingMethodAdapter GetAdapter(ShippingMethod method)
        {
            switch (method)
            {
                case ShippingMethod.NoShipping:
                    return GetDecoratedAdapter(new NoShippingMethodAdapter());
                case ShippingMethod.ActualNoShipping:
                    return GetDecoratedAdapter(new ActualNoShippingMethodAdapter());
                case ShippingMethod.PrivateFleet:
                    return GetDecoratedAdapter(new PrivateFleetMethodAdapter());
                case ShippingMethod.OnSitePickup:
                    return GetDecoratedAdapter(new OnSitePickUpMethodAdapter());
                case ShippingMethod.PostalExpress:
                    return GetDecoratedAdapter(new SedexMethodAdapter());
                case ShippingMethod.PostalRegular:
                    return GetDecoratedAdapter(new PacMethodAdapter());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IShippingMethodAdapter GetDecoratedAdapter(IShippingMethodAdapter adapter)
                => new ShippingMethodAdapterCacheDecorator(cache, adapter);

    }
}
