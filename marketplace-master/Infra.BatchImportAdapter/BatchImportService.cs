using Application.Modules.BatchImport;
using Application.Modules.BatchImport.Models;
using Application.Modules.BatchImport.Sellers;
using Domain.Entities;
using Domain.Modules.BatchImport;
using Infra.BatchImportAdapter.Internals;
using Infra.BatchImportAdapter.Products;
using Infra.BatchImportAdapter.Sellers;
using System.IO;
using System.Threading.Tasks;

namespace Infra.BatchImportAdapter
{
    public class BatchImportService : IBatchImportService
    {
        private readonly CustomFieldsAdapter customFieldsAdapter;

        public BatchImportService(CustomFieldsAdapter customFieldsAdapter)
        {
            this.customFieldsAdapter = customFieldsAdapter;
        }

        public async Task GenerateProductTemplateAsync(BatchImportSourceType sourceType, Stream output)
        {
            var customFields = new CustomFieldDto[0];
            var dataModel = new ProductModelAndParser(customFields);
            await InternalImportService.GenerateTemplateAsync(sourceType, dataModel, output);
        }

        public async Task GenerateSellerTemplateAsync(BatchImportSourceType sourceType, Stream output)
        {
            var customFields = await customFieldsAdapter.GetCustomFieldsAsync(CustomFieldEntityType.Seller);
            var dataModel = new SellerModelAndParser(customFields);
            await InternalImportService.GenerateTemplateAsync(sourceType, dataModel, output);
        }

        public async Task<ImportResult<ProductData>> ImportProductsAsync(BatchImportSourceType sourceType, Stream input)
        {
            var customFields = new CustomFieldDto[0];
            var dataModel = new ProductModelAndParser(customFields);
            return await InternalImportService.ImportAsync(dataModel, sourceType, input);
        }

        public async Task<ImportResult<SellerDto>> ImportSellersAsync(BatchImportSourceType sourceType, Stream input)
        {
            var customFields = await customFieldsAdapter.GetCustomFieldsAsync(CustomFieldEntityType.Seller);
            var dataModel = new SellerModelAndParser(customFields);
            return await InternalImportService.ImportAsync(dataModel, sourceType, input);
        }
    }
}
