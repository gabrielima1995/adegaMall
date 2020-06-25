using Domain.Entities.StoreModule;
using System.Collections.ObjectModel;

namespace Application.Modules.Products.Queries
{
    public interface IProductSearchQueries
    {
        ReadOnlyCollection<Product> ListAll();
    }
}
