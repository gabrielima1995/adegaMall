using Domain.Entities.StoreModule;

namespace Domain.Interfaces
{
    public interface IShippingService
    {
        IShippingMethodAdapter GetAdapter(ShippingMethod method);
    }
}
