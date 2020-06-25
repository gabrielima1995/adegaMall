using Application.Modules.BatchImport.Models;
using Domain.Modules.BatchImport;
using System.IO;
using System.Threading.Tasks;

namespace Application.Modules.BatchImport.Products
{
    public class ProductsImportAppService
    {
        private readonly GenerateProductTemplateCommandHandler templateCommandHandler;
        private readonly ImportProductsCommandHandler importProductsCommandHandler;
        private readonly ImportProductImagesCommandHandler importProductImagesCommandHandler;

        public ProductsImportAppService(
            GenerateProductTemplateCommandHandler templateCommandHandler,
            ImportProductsCommandHandler importProductsCommandHandler,
            ImportProductImagesCommandHandler importProductImagesCommandHandler)
        {
            this.templateCommandHandler = templateCommandHandler;
            this.importProductsCommandHandler = importProductsCommandHandler;
            this.importProductImagesCommandHandler = importProductImagesCommandHandler;
        }

        public async Task<Stream> GenerateProductTemplateAsync(GenerateProductTemplateCommand command)
        {
            return await templateCommandHandler.HandleAsync(command);
        }

        public async Task<ImportResult<ProductData>> ImportProductsAsync(ImportProductsCommand command)
        {
            return await importProductsCommandHandler.HandleAsync(command);
        }

        public async Task<ImportResult<string>> ImportProductImagesAsync(ImportProductImagesCommand command)
        {
            return await importProductImagesCommandHandler.HandleAsync(command);
        }
    }
}
