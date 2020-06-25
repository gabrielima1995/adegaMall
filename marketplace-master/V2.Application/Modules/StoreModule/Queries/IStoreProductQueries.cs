using Domain.DTO;

namespace V2.Application.Modules.StoreModule.Queries
{
    public interface IStoreProductQueries
    {
        StoreProductResultsVM ListProducts(string sellerId, string query, int startIndex, int pageSize);
    }
}
