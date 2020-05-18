namespace V2.Application.Modules.Search.Sellers
{
    public interface ISellerSearchService
    {
        CommonSearchResult Search(CommonSearchRequest request);
    }
}