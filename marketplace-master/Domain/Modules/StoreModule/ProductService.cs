using Application.Config;
using Domain.DTO;
using Domain.Entities.StoreModule;
using Domain.Events.SellerProfileUpdated;
using Domain.Events.StoreModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Modules.Products.StockManagement;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Domain.Services.StoreModule
{
    public class ProductService
    {
        private readonly ICloudStorage _cloudStorage;
        private readonly IProductRepository _productRepository;
        private readonly StockTransactionService _stockTransactionService;

        public ProductService(
            ICloudStorage cloudStorage,
            IProductRepository productRepository,
            StockTransactionService stockTransactionService)
        {
            _cloudStorage = cloudStorage;
            _productRepository = productRepository;
            _stockTransactionService = stockTransactionService;
        }

        public Product GetById(Guid id)
        {
            return _productRepository.GetById(id);
        }

        public Product GetByRef(string customId)
        {
            return _productRepository.GetByRef(customId);
        }

        public ProductDetailsForPublicPage GetDetailsForPublicPage(Guid id)
        {
            var requireGatewayId = MaaSConfig.Current.Payment.Enabled;
            var requireActiveMembership = MaaSConfig.Current.Subscription.IsRequired;

            var product = _productRepository.GetDetailsForPublicPage(id, requireGatewayId, requireActiveMembership);
            if (product == null) return null;

            return new ProductDetailsForPublicPage
            {
                Product = product,
                PaymentOptions = GetPaymentOptions(product)
            };
        }

        public List<Product> ListForEcommerceHome(int productCategoryId)
        {
            var requireGatewayId = MaaSConfig.Current.Payment.Enabled;
            var requireActiveMembership = MaaSConfig.Current.Subscription.IsRequired;
            return _productRepository.ListForEcommerceHome(productCategoryId, requireGatewayId, requireActiveMembership);
        }

        public List<Product> ListTopForSeller(string sellerId)
        {
            return _productRepository.ListTopForSeller(sellerId);
        }

        public OperationResult Create(Product product, List<FileUpload> images)
        {
            return CreateUpdateInternal(product, images, isUpdate: false);
        }

        public OperationResult Delete(Product product)
        {
            product.Deleted = true;
            product.DeletedAt = DateTime.UtcNow;
            product.AddDomainEvent(new ProductDeletedEvent(product.Id, product.Store_SellerId));

            var result = _productRepository.Update(product, saveChanges: true);
            if (!result.OK) return result;

            product.AddDomainEvent(new SellerProfileUpdated(product.Store_SellerId));
            return OperationResult.Success;
        }

        public OperationResult Update(Product product, List<FileUpload> images)
        {
            return CreateUpdateInternal(product, images, isUpdate: true);
        }

        public OperationResult Update(ProductEditing model, string sellerId)
        {
            var product = model.Id != Guid.Empty ? GetById(model.Id) : GetByRef(model.CustomId);

            if (string.IsNullOrEmpty(sellerId) || sellerId != product.Store_SellerId)
                return OperationResult.Fail("InvalidSeller", $"The provided authentication token is invalid or does not match the one for the seller to whom the product {product.Id} is registered.");

            if (model.CustomId != null) product.CustomId = model.CustomId;
            if (model.Price != null) product.SetPrice(model.Price.Value);
            if (model.Title != null) product.Title = model.Title;
            if (!string.IsNullOrEmpty(model.Description)) product.Description = model.Description;
            if (!string.IsNullOrEmpty(model.Specifications)) product.Specifications = model.Specifications;
            if (model.CurrentStock != null) product.CurrentStock = model.CurrentStock.Value;

            var updateResult = Update(product, null);
            if (updateResult.IsFailure)
                return new OperationResult(updateResult.Errors);

            return new OperationResult();
        }

        public UpdateFromExternalSourceResult UpdateFromExternalSource()
        {
            var result = new UpdateFromExternalSourceResult();

            return result;
        }

        private OperationResult CreateUpdateInternal(Product product, List<FileUpload> images, bool isUpdate)
        {
            Sanitize(product);

            if (images?.Any() != null && images.Any())
            {
                bool coverImageSet = false;

                foreach (var img in images.Where(x => x.ContentLength > 0))
                {
                    try
                    {
                        var uri = _cloudStorage.UploadFile(CloudFileType.ProductImage.ToString(), img.FileName, img.ContentType, img.InputStream);

                        if (!coverImageSet)
                        {
                            product.CoverImageUrl = uri.ToString();
                            coverImageSet = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        return OperationResult.Fail("", ex.Message);
                    }
                }
            }

            if (isUpdate)
            {
                product.AddDomainEvent(new ProductUpdatedEvent(product, product.Store_SellerId));

                var updateResult = _productRepository.Update(product, saveChanges: true);
                if (!updateResult.OK) return updateResult;
            }
            else
            {
                product.AddDomainEvent(new ProductAddedEvent(product, product.Store_SellerId, product.Title, (int)product.CurrentStock));

                var addResult = _productRepository.Add(product, saveChanges: true);
                if (!addResult.OK) return addResult;
            }

            return OperationResult.Success;
        }

        private ProductPaymentOptions GetPaymentOptions(Product product)
        {
            if (product == null || (product.Price ?? 0M) == 0M)
                return null;

            var config = MaaSConfig.Current;
            var price = product.BasePrice.Value;

            var dto = new ProductPaymentOptions
            {
                BoletoEnabled = config.Payment.Enabled && config.Payment.PaymentGateway.Boleto.Enabled,
                CreditCardEnabled = config.Payment.Enabled && config.Payment.PaymentGateway.CreditCard.Enabled
            };

            if (dto.CreditCardEnabled)
            {
                var installmentsConfig = config.Payment.PaymentGateway.Installments;

                if (price >= installmentsConfig.MinOrderAmount)
                {
                    var maxInstallments = price / Math.Max(installmentsConfig.MinInstallmentAmount, 1M);
                    maxInstallments = Math.Min(maxInstallments, installmentsConfig.MaxInstallments);

                    if (product.MaxInstallments.HasValue)
                        maxInstallments = Math.Min(maxInstallments, product.MaxInstallments.Value);

                    maxInstallments = Math.Max(Math.Floor(maxInstallments), 1M);
                    dto.MaxInstallments = (int)maxInstallments;
                    dto.InstallmentAmount = (price / dto.MaxInstallments).RoundTo2Places();

                    if (config.Payment.GatewayIsWirecard() && config.Payment.Moip.Options.CustomerPaysFees)
                    {
                        var transactionFee = CalculateOrderInstallmentFeesForWirecard(dto.MaxInstallments);
                        var anticipationFee = config.Payment.Moip.Options.FeesConfig.AnticipationFees[dto.MaxInstallments] / 100M;

                        var installmentFees = price * ((1 - transactionFee) / (1 - transactionFee - anticipationFee)) - price;
                        dto.InstallmentAmount = ((price + installmentFees) / dto.MaxInstallments).RoundTo2Places();
                    }
                }
            }

            return dto;
        }

        private decimal CalculateOrderInstallmentFeesForWirecard(int numberOfInstallments)
        {
            var config = MaaSConfig.Current;
            if (config.Payment.Moip.Options.FeesConfig.InstallmentFees.Any(x => x > 0))
                return config.Payment.Moip.Options.FeesConfig.InstallmentFees[numberOfInstallments - 1] / 100m;
            else
                return config.Payment.Moip.Options.FeesConfig.BaseFee / 100m;
        }

        private void Sanitize(Product product)
        {
            StringHelper.Sanitize(product);
        }
    }

    [Obsolete("Use Domain.Modules.Products.ProductService instead.")]
    public class ProductService_Obsolete : ServiceBase<Product>
    {
        private readonly StockTransactionService _stockTransactionService;

        public ProductService_Obsolete(StockTransactionService stockTransactionService, IRepository<Product> repository)
            : base(repository)
        {
            _stockTransactionService = stockTransactionService;
        }

        public override Product Save(Expression<Func<Product, bool>> get, Func<Product> create, Action<Product> update, params Expression<Func<Product, object>>[] includes)
        {
            var product = Get(get, includes);

            if (product == null)
            {
                product = create();

                product.CreatedAt = DateTime.UtcNow;
                product.AddDomainEvent(new ProductAddedEvent(product, product.Store_SellerId, product.Title, (int)product.CurrentStock));
                product.AddDomainEvent(new SellerProfileUpdated(product.Store_SellerId));

                Repository.Insert(product);
                Repository.SaveChanges();
            }
            else
            {
                update(product);

                product.UpdatedAt = DateTime.UtcNow;
                product.AddDomainEvent(new ProductUpdatedEvent(product, product.Store_SellerId));

                Repository.Update(product);
                Repository.SaveChanges();
            }

            return product;
        }
    }
}