using Domain.Entities.StoreModule;
using System.Collections.Generic;

namespace Domain.Interfaces.Repositories
{
    public interface IStoreRepository : ICrudRepository<Store>
    {
        Store GetForEditing(string sellerId);

        List<Product> ListByStore(string sellerId);

        void Add(Store store);

        bool ExistsBySellerId(string sellerId);
    }
}
