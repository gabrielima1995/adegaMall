namespace V2.Application.Modules.Search.Quotes
{
    public interface IQuoteSearchService
    {
        QuoteSearchResult Search(QuoteSearchRequest request);
        CommonSearchResult Search(CommonSearchRequest request);
    }
}