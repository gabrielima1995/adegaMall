using Application.Modules.BatchImport.Models;
using Application.Modules.BatchImport.Sellers;
using Domain.Modules.BatchImport;
using System.IO;
using System.Threading.Tasks;

namespace Application.Modules.BatchImport
{
    public interface IBatchImportService
    {
        Task GenerateSellerTemplateAsync(BatchImportSourceType sourceType, Stream output);
        Task GenerateProductTemplateAsync(BatchImportSourceType sourceType, Stream output);
        Task<ImportResult<ProductData>> ImportProductsAsync(BatchImportSourceType sourceType, Stream input);
        Task<ImportResult<SellerDto>> ImportSellersAsync(BatchImportSourceType sourceType, Stream input);
    }
}
