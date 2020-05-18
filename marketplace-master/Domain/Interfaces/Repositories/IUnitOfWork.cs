using Domain.Modules.Finance.Interfaces;
using Domain.StoreModule.Interfaces;
using Framework;

namespace Domain.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IAdminReportRepository AdminReportRepository { get; }

        IApiTokenRepository ApiTokenRepository { get; }

        IApplicationConfigRepository ApplicationConfigRepository { get; }

        IBannerRepository BannerRepository { get; }

        IBuyerRepository BuyerRepository { get; }

        ICartRepository CartRepository { get; }

        IEcommerceHomeCategoryRepository EcommerceHomeCategoryRepository { get; }

        IEcommerceHomeRepository EcommerceHomeRepository { get; }

        IOrderRepository OrderRepository { get; }

        IOrderTransactionRepository OrderTransactionRepository { get; }

        IPersonRepository PersonRepository { get; }

        IPlanRepository PlanRepository { get; }

        IPortfolioCategoryRepository PortfolioCategoryRepository { get; }

        IPortfolioRepository PortfolioRepository { get; }

        IPortfolioSubCategoryRepository PortfolioSubCategoryRepository { get; }

        IProductCategoryRepository ProductCategoryRepository { get; }

        IProductRepository ProductRepository { get; }

        IProductVariantRepository ProductVariantRepository { get; }

        IQuoteAnswerRepository QuoteAnswerRepository { get; }

        IQuoteRepository QuoteRepository { get; }

        ISellerRepository SellerRepository { get; }

        IStockTransactionRepository StockTransactionRepository { get; }

        IStoreRepository StoreRepository { get; }

        IStringRepository StringRepository { get; }

        ITransferJournalRepository TransferJournalRepository { get; }

        IWebhookRepository WebhookRepository { get; }

        OperationResult Save();
    }
}
