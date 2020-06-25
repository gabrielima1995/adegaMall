using Application.Modules.BatchImport.Events;
using Application.Modules.BatchImport.Models;
using Application.Modules.Products.Categories;
using Application.Modules.Products.Store;
using Domain.Core;
using Domain.Modules.BatchImport;
using Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Modules.BatchImport.Products
{
    public class ImportProductsCommand
    {
        public ImportProductsCommand(BatchImportSourceType sourceType, Stream input)
        {
            SourceType = sourceType;
            Input = input;
        }

        public BatchImportSourceType SourceType { get; }
        public Stream Input { get; }
    }

    public class ImportProductsCommandHandler
    {
        private readonly IDomainEventDispatcher domainEventDispatcher;
        private readonly IBatchImportService batchImportService;
        private readonly ProductCategoryAppService productCategoryAppService;
        private readonly IProductImportQueries productImportQueries;
        private readonly StoreAppService storeAppService;

        public ImportProductsCommandHandler(
            IDomainEventDispatcher domainEventDispatcher,
            IBatchImportService batchImportService,
            ProductCategoryAppService productCategoryAppService,
            IProductImportQueries productImportQueries,
            StoreAppService storeAppService)
        {
            this.domainEventDispatcher = domainEventDispatcher;
            this.batchImportService = batchImportService;
            this.productCategoryAppService = productCategoryAppService;
            this.productImportQueries = productImportQueries;
            this.storeAppService = storeAppService;
        }

        public async Task<ImportResult<ProductData>> HandleAsync(ImportProductsCommand command)
        {
            var parseResult = await batchImportService.ImportProductsAsync(command.SourceType, command.Input);
            var processResult = ProcessProducts(parseResult.FieldNames, parseResult.ValidRecords);

            var invalidRecords = parseResult.InvalidRecords.ToList();
            invalidRecords.AddRange(processResult.InvalidRecords);

            return new ImportResult<ProductData>(
                parseResult.FieldNames,
                processResult.ValidRecords,
                invalidRecords);
        }

        private ImportResult<ProductData> ProcessProducts(IEnumerable<string> fieldNames, IEnumerable<ProductData> products)
        {
            var productsWithCategories = ProcessCategories(products);
            var processResults = new List<ProcessProductResult>(products.Count());

            var databaseProductIds = productImportQueries.ListProductIdsByCustomIds(products.Select(p => new CustomIdAndSellerEmail(p.CustomId, p.SellerEmail)));
            var databaseSellerIds = productImportQueries.ListSellerIdsBySellerEmails(products.Select(p => p.SellerEmail).Distinct());

            foreach (var product in products)
            {
                var result = ProcessProduct(databaseProductIds, databaseSellerIds, product);
                processResults.Add(result);
            }

            var successfulProducts = processResults.Where(result => result.IsSuccess).Select(result => result.Product);
            var invalidProducts = processResults.Where(result => result.IsFailure).Select(result => result.InvalidProduct);

            return new ImportResult<ProductData>(fieldNames, successfulProducts, invalidProducts);
        }

        private IEnumerable<ProductData> ProcessCategories(IEnumerable<ProductData> products)
        {
            var categoryNames = products.Select(product => product.CategoryName).Distinct(StringComparer.OrdinalIgnoreCase);
            var categories = ProcessCategoriesInternal(categoryNames);

            foreach (var product in products)
            {
                var category = categories.Single(cat => cat.Name.Equals(product.CategoryName, StringComparison.OrdinalIgnoreCase));
                product.CategoryId = category.Id;
            }

            return products;
        }

        private IEnumerable<ProductCategoryViewModel> ProcessCategoriesInternal(IEnumerable<string> categoryNames)
        {
            var existingCategories = productCategoryAppService.GetList();
            var existingNames = existingCategories.Select(category => category.Name);

            var newNames = categoryNames.Except(existingNames, StringComparer.OrdinalIgnoreCase);
            var newCategories = newNames.Select(name => new ProductCategoryViewModel { Name = name });

            foreach (var viewModel in newCategories)
            {
                var saveResult = productCategoryAppService.SaveCategory(viewModel);
                viewModel.Id = saveResult.Result.Id;
            }

            var allCategories = existingCategories.ToList();
            allCategories.AddRange(newCategories);

            return allCategories;
        }

        private ProcessProductResult ProcessProduct(
            ICollection<KeyValuePair<string, Guid>> databaseProductIds,
            ICollection<KeyValuePair<string, string>> databaseSellerIds,
            ProductData product)
        {
            var viewModel = product.AsProductViewModel();
            viewModel.Id = AttemptGetProductIdForDto(databaseProductIds, product);
            viewModel.SellerId = AttemptGetSellerIdForDto(databaseSellerIds, product);

            if (viewModel.SellerId == null)
            {
                var errors = new ErrorDictionary(nameof(ProductData.SellerEmail), "Não encontramos um cadastro de vendedor com este email.");
                var invalidProduct = product.ToInvalidImportProduct(errors);
                return ProcessProductResult.Failure(invalidProduct);
            }

            var isNewProduct = viewModel.Id == Guid.Empty;
            var saveResult = storeAppService.SaveProduct(viewModel);

            if (saveResult.OK)
            {
                if (isNewProduct)
                    domainEventDispatcher.Dispatch(new ProductImportedEvent(saveResult.Result));

                return ProcessProductResult.Success(product);
            }
            else
            {
                var invalidProduct = product.ToInvalidImportProduct(saveResult.Errors);
                return ProcessProductResult.Failure(invalidProduct);
            }
        }

        private Guid AttemptGetProductIdForDto(ICollection<KeyValuePair<string, Guid>> databaseProductIds, ProductData product)
        {
            var productId = databaseProductIds.FirstOrDefault(entry => entry.Key.Equals(product.CustomId, StringComparison.OrdinalIgnoreCase));
            return productId.Value;
        }

        private string AttemptGetSellerIdForDto(ICollection<KeyValuePair<string, string>> databaseSellerIds, ProductData product)
        {
            var sellerId = databaseSellerIds.FirstOrDefault(entry => entry.Key.Equals(product.SellerEmail, StringComparison.OrdinalIgnoreCase));
            return sellerId.Value;
        }

        private class ProcessProductResult
        {
            private ProcessProductResult(ProductData product, InvalidImportProduct invalidProduct)
            {
                Product = product;
                InvalidProduct = invalidProduct;
            }

            internal static ProcessProductResult Success(ProductData product) => new ProcessProductResult(product, null);
            internal static ProcessProductResult Failure(InvalidImportProduct invalidProduct) => new ProcessProductResult(null, invalidProduct);

            public ProductData Product { get; }
            public InvalidImportProduct InvalidProduct { get; }
            public bool IsSuccess => Product != null;
            public bool IsFailure => !IsSuccess;
        }
    }
}
