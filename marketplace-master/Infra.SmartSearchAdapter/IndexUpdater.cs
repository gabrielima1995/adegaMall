namespace Infra.SmartSearchAdapter
{
    public class IndexUpdater
    {
        private readonly QuoteIndexHelper quoteIndexHelper;
        private readonly SellerIndexHelper sellerIndexHelper;
        private readonly ProductIndexHelper productIndexHelper;

        public IndexUpdater(
            QuoteIndexHelper quoteIndexHelper,
            SellerIndexHelper sellerIndexHelper,
            ProductIndexHelper productIndexHelper)
        {
            this.quoteIndexHelper = quoteIndexHelper;
            this.sellerIndexHelper = sellerIndexHelper;
            this.productIndexHelper = productIndexHelper;
        }

        public void CreateOrUpdateIndexes()
        {
            IndexQuotes();
            IndexSellers();
            IndexProducts();
        }

        private void IndexQuotes() => quoteIndexHelper.CreateOrUpdateIndex();

        private void IndexSellers() => sellerIndexHelper.CreateOrUpdateIndex();

        private void IndexProducts() => productIndexHelper.CreateOrUpdateIndex();
    }
}