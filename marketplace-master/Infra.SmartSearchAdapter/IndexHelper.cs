using Domain.Modules.StoreModule;
using Infra.SmartSearchAdapter.Products;
using Infra.SmartSearchAdapter.Quotes;
using Infra.SmartSearchAdapter.Sellers;
using SmartSearch;
using SmartSearch.Abstractions;
using SmartSearch.LuceneNet;
using System;

namespace Infra.SmartSearchAdapter
{
    public interface IIndexHelper
    {
        void CreateOrUpdateIndex();

        void AddItem(object itemId);

        void RemoveItem(object itemId);

        void UpdateItem(object itemId);
    }

    public abstract class IndexHelperBase : IIndexHelper
    {
        public IIndexContext IndexContext { get; protected set; }

        public ISearchDomain SearchDomain { get; protected set; }

        public IndexHelperBase(IIndexContext indexContext, ISearchDomain searchDomain)
        {
            IndexContext = indexContext ?? throw new ArgumentNullException(nameof(indexContext));
            SearchDomain = searchDomain ?? throw new ArgumentNullException(nameof(searchDomain));
        }

        public abstract void CreateOrUpdateIndex();

        public abstract void AddItem(object itemId);

        public void RemoveItem(object itemId)
        {
            var documentId = itemId.ToString();
            var deleteOperation = DocumentOperation.Delete(documentId);
            var documentProvider = new DocumentProvider(new[] { deleteOperation });

            CreateIndex(documentProvider);
        }

        public abstract void UpdateItem(object itemId);

        protected virtual void CreateIndex(IDocumentProvider documentProvider)
        {
            try
            {
                new LuceneIndexService(new DefaultIndexOptions()).CreateIndex(
                    IndexContext, SearchDomain, documentProvider);
            }
            catch (Exception)
            {
#if !DEBUG
                throw;
#endif
            }
        }
    }

    public class QuoteIndexHelper : IndexHelperBase
    {
        public QuoteIndexHelper() : base(new QuoteIndexContext(), new QuoteSearchDomain())
        {
        }

        public override void AddItem(object itemId) => CreateIndex(new QuoteDocumentProvider((Guid)itemId));

        public override void CreateOrUpdateIndex() => CreateIndex(new QuoteDocumentProvider());

        public override void UpdateItem(object itemId) => CreateIndex(new QuoteDocumentProvider((Guid)itemId));
    }

    public class SellerIndexHelper : IndexHelperBase
    {
        public SellerIndexHelper() : base(new SellerIndexContext(), new SellerSearchDomain())
        {
        }

        public override void AddItem(object itemId) => CreateIndex(new SellerDocumentProvider((string)itemId));

        public override void CreateOrUpdateIndex() => CreateIndex(new SellerDocumentProvider());

        public override void UpdateItem(object itemId) => CreateIndex(new SellerDocumentProvider((string)itemId));
    }

    public class ProductIndexHelper : IndexHelperBase
    {
        private readonly RentalSearchDocumentService rentalSearchDocumentService;

        public ProductIndexHelper(RentalSearchDocumentService rentalSearchDocumentService) : base(new ProductIndexContext(), new ProductSearchDomain())
        {
            this.rentalSearchDocumentService = rentalSearchDocumentService;
        }

        public override void AddItem(object itemId) => CreateIndex(new ProductDocumentProvider(rentalSearchDocumentService, Guid.Parse(itemId.ToString())));

        public override void CreateOrUpdateIndex() => CreateIndex(new ProductDocumentProvider(rentalSearchDocumentService));

        public override void UpdateItem(object itemId) => CreateIndex(new ProductDocumentProvider(rentalSearchDocumentService, Guid.Parse(itemId.ToString())));

        public void UpdateItemsBySeller(string sellerId) => CreateIndex(new ProductDocumentProvider(rentalSearchDocumentService, sellerId: sellerId));
    }
}