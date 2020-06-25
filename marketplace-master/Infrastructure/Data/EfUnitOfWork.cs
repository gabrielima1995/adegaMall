using Domain.Interfaces.Repositories;
using Domain.Modules.Finance.Interfaces;
using Domain.StoreModule.Interfaces;
using Framework;
using Infrastructure.Data.Context;
using System;
using System.Data.Entity.Validation;
using System.Linq;

namespace Infrastructure.Data
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly ScaffoldContext context;

        public IAdminReportRepository AdminReportRepository { get; }
        public IApiTokenRepository ApiTokenRepository { get; }
        public IApplicationConfigRepository ApplicationConfigRepository { get; }
        public IBannerRepository BannerRepository { get; }
        public IBuyerRepository BuyerRepository { get; }
        public ICartRepository CartRepository { get; }
        public IEcommerceHomeCategoryRepository EcommerceHomeCategoryRepository { get; }
        public IEcommerceHomeRepository EcommerceHomeRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IOrderTransactionRepository OrderTransactionRepository { get; }
        public IPersonRepository PersonRepository { get; }
        public IPlanRepository PlanRepository { get; }
        public IPortfolioCategoryRepository PortfolioCategoryRepository { get; }
        public IPortfolioRepository PortfolioRepository { get; }
        public IPortfolioSubCategoryRepository PortfolioSubCategoryRepository { get; }
        public IProductCategoryRepository ProductCategoryRepository { get; }
        public IProductVariantRepository ProductVariantRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IQuoteAnswerRepository QuoteAnswerRepository { get; }
        public IQuoteRepository QuoteRepository { get; }
        public ISellerRepository SellerRepository { get; }
        public IStockTransactionRepository StockTransactionRepository { get; }
        public IStoreRepository StoreRepository { get; }
        public IStringRepository StringRepository { get; }
        public ITransferJournalRepository TransferJournalRepository { get; }
        public IWebhookRepository WebhookRepository { get; }

        public EfUnitOfWork(ScaffoldContext context,
            IAdminReportRepository adminReportRepository,
            IApiTokenRepository apiTokenRepository,
            IApplicationConfigRepository applicationConfigRepository,
            IBannerRepository bannerRepository,
            IBuyerRepository buyerRepository,
            ICartRepository cartRepository,
            IEcommerceHomeCategoryRepository ecommerceHomeCategoryRepository,
            IEcommerceHomeRepository ecommerceHomeRepository,
            IOrderRepository orderRepository,
            IOrderTransactionRepository orderTransactionRepository,
            IPersonRepository personRepository,
            IPlanRepository planRepository,
            IPortfolioCategoryRepository portfolioCategoryRepository,
            IPortfolioRepository portfolioRepository,
            IPortfolioSubCategoryRepository portfolioSubCategoryRepository,
            IProductCategoryRepository productCategoryRepository,
            IProductRepository productRepository,
            IProductVariantRepository productVariantRepository,
            IQuoteAnswerRepository quoteAnswerRepository,
            IQuoteRepository quoteRepository,
            ISellerRepository sellerRepository,
            IStockTransactionRepository stockTransactionRepository,
            IStoreRepository storeRepository,
            IStringRepository stringRepository,
            ITransferJournalRepository transferJournalRepository,
            IWebhookRepository webhookRepository)
        {
            this.context = context;

            AdminReportRepository = adminReportRepository;
            ApiTokenRepository = apiTokenRepository;
            ApplicationConfigRepository = applicationConfigRepository;
            BannerRepository = bannerRepository;
            BuyerRepository = buyerRepository;
            CartRepository = cartRepository;
            EcommerceHomeCategoryRepository = ecommerceHomeCategoryRepository;
            EcommerceHomeRepository = ecommerceHomeRepository;
            OrderRepository = orderRepository;
            OrderTransactionRepository = orderTransactionRepository;
            PersonRepository = personRepository;
            PlanRepository = planRepository;
            PortfolioCategoryRepository = portfolioCategoryRepository;
            PortfolioRepository = portfolioRepository;
            PortfolioSubCategoryRepository = portfolioSubCategoryRepository;
            ProductCategoryRepository = productCategoryRepository;
            ProductVariantRepository = productVariantRepository;
            ProductRepository = productRepository;
            QuoteAnswerRepository = quoteAnswerRepository;
            QuoteRepository = quoteRepository;
            SellerRepository = sellerRepository;
            StockTransactionRepository = stockTransactionRepository;
            StoreRepository = storeRepository;
            StringRepository = stringRepository;
            TransferJournalRepository = transferJournalRepository;
            WebhookRepository = webhookRepository;
        }

        public OperationResult Save()
        {
            try
            {
                if (context.ChangeTracker.HasChanges())
                    context.SaveChanges();

                return OperationResult.Success;
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);
                return new OperationResult("", exceptionMessage);
            }
            catch (Exception ex)
            {
                return new OperationResult("", ex.Message);
            }
        }
    }
}
