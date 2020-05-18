using Application.Config;
using Domain.DTO;
using Domain.Entities.StoreModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.QuoteBillingModule.Entities;
using Domain.Modules.QuoteBillingModule.Interfaces;
using Domain.Services.StoreModule;
using Infra.Crosscutting.Globalization;
using System;
using V2.Application.Modules.QuoteBillingModule.Queries;

namespace V2.Application.Modules.QuoteBillingModule
{
    public class QuoteBillingAppService
    {
        private readonly CartService _cartService;
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IQuoteBillingQueries _quoteBillingQueries;
        private readonly IQuoteBillingRepository _quoteBillingRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuoteBillingAppService(
            CartService cartService,
            IProductRepository productRepository,
            IStoreRepository storeRepository,
            IQuoteBillingQueries quoteBillingQueries,
            IQuoteBillingRepository quoteBillingRepository,
            IQuoteRepository quoteRepository,
            IUnitOfWork unitOfWork)
        {
            _cartService = cartService;
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _quoteBillingQueries = quoteBillingQueries;
            _quoteBillingRepository = quoteBillingRepository;
            _quoteRepository = quoteRepository;
            _unitOfWork = unitOfWork;
        }

        public void CancelQuoteBilling(Guid quoteBillingId)
        {
            var billing = _quoteBillingRepository.Get(quoteBillingId);

            if (billing == null)
                throw new Exception($"Quote billing '{quoteBillingId}' does not exist. Cannot cancel quote billing.");

            billing.MarkCancelled();

            if (billing.ProductId != null)
                DeleteBillingProduct(billing);

            _quoteBillingRepository.Update(billing);
            _unitOfWork.Save();
        }

        public QuoteBilling CreateBillingFromQuote(Guid quoteId)
        {
            if (_quoteBillingRepository.GetForQuote(quoteId) != null)
                throw new InvalidOperationException($"Quote '{quoteId}' already has a billing object. Cannot have two billings for a single quote.");

            var quote = _quoteRepository.GetById(quoteId);

            if (!quote.IsClosed)
                throw new InvalidOperationException($"Cannot create billing for quote '{quoteId}' because it is not closed.");

            if (quote.AcceptedPrice == null)
                throw new ArgumentException($"Cannot create billing for quote '{quoteId}' because it doesn't have an accepted price.");

            if (quote.Deadline == null)
                throw new ArgumentException($"Cannot create billing for quote '{quoteId}' because it doesn't have an accepted deadline.");

            var billing = QuoteBilling.CreateBilling(Guid.NewGuid(), quoteId,
                quote.AcceptedPrice.Value, quote.Deadline.Value, quote.Seller_Id, quote.Buyer_Id);

            EnsureSellerHasStore(quote.Seller_Id);

            _quoteBillingRepository.Add(billing);
            _unitOfWork.Save();

            return billing;
        }

        public Checkout GetChekoutForBilling(Guid quoteBillingId)
        {
            var billing = _quoteBillingRepository.Get(quoteBillingId);

            if (billing == null)
                throw new Exception($"Quote billing '{quoteBillingId}' does not exist. Cannot create checkout.");

            if (billing.ProductId == null)
                CreateBillingProduct(billing);

            _cartService.RemoveAllItems(billing.BuyerId);
            _cartService.AddItemWithoutShipping(billing.BuyerId, billing.ProductId.Value, 1, null, null, 0, null, null, isQuote: true);
            _unitOfWork.Save();

            return _cartService.GetCheckout(billing.BuyerId);
        }

        private void CreateBillingProduct(QuoteBilling billing)
        {
            var quote = _quoteRepository.GetById(billing.QuoteId);

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Store_SellerId = billing.SellerId,
                Title = GlobalizationHelper.GetString("PRODUCT_NAME_FOR_QUOTE_BILLING"),
                Description = GlobalizationHelper.FormatString("PRODUCT_DESCRIPTION_FOR_QUOTE_BILLING", quote.Title),
                Specifications = GlobalizationHelper.GetString("PRODUCT_SPECIFICATIONS_FOR_QUOTE_BILLING"),
                FriendlyUrl = "quote-billing-" + billing.Id.Substring(),
                CoverImageUrl = MaaSConfig.Current.Layout.Images.BillingDummyImage,
                CurrentStock = 1,
                DeleteWhenStockZero = true,
                ActualNoShippingEnabled = true,
                IsBillingProduct = true,
            };

            product.SetPrice(billing.Amount);
            billing.SetProductId(product.Id);

            _productRepository.Add(product);
            _quoteBillingRepository.Update(billing);
        }

        private void EnsureSellerHasStore(string sellerId)
        {
            if (_storeRepository.ExistsBySellerId(sellerId))
                return;

            _storeRepository.Add(
                new Store { SellerId = sellerId });
        }

        private void DeleteBillingProduct(QuoteBilling billing)
        {
            if (billing.ProductId == null)
                return;

            var product = _productRepository.GetById(billing.ProductId);

            product.CurrentStock = 0;
            product.Deleted = true;
            product.DeletedAt = DateTime.UtcNow;

            billing.ClearProductId();

            _quoteBillingRepository.Update(billing);
            _productRepository.Update(product);
        }
    }
}
