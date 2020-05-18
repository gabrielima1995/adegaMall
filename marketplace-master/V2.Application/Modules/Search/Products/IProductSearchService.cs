using System.Collections.Generic;
using V2.Application.Modules.StoreModule.Queries;

namespace V2.Application.Modules.Search.Products
{
    public interface IProductSearchService
    {
        ProductSearchResult Search(ProductSearchRequest request);

        CommonSearchResult Search(CommonSearchRequest request);

        ProductSearchResult Nearby(NearbyProductsRequest request);

        List<RentalScheduleTimelineResourceItem> ScheduleTimelineProfile(string sellerId);
    }
}