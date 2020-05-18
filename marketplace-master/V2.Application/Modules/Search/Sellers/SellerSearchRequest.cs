namespace V2.Application.Modules.Search.Sellers
{
    public class SellerSearchRequest : SearchRequestBase
    {
        public SellerSort Sort = SellerSort.MostRelevant;

        public int? CategoryId { get; set; }

        public int? SubCategoryId { get; set; }
    }
}