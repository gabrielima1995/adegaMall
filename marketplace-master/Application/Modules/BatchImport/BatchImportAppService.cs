using Application.Modules.BatchImport.Models;
using Application.Modules.BatchImport.Sellers;
using System.IO;
using System.Threading.Tasks;

namespace Application.Modules.BatchImport
{
    public class BatchImportAppService
    {
        private readonly SellersImportAppService sellersImportAppService;

        public BatchImportAppService(SellersImportAppService sellersImportAppService)
        {
            this.sellersImportAppService = sellersImportAppService;
        }

        public async Task GenerateSellerTemplateAsync(BatchImportSourceType sourceType, Stream output)
        {
            await sellersImportAppService.GenerateSellerTemplateAsync(sourceType, output);
        }

        public async Task<ImportResult<SellerDto>> ImportSellersAsync(BatchImportSourceType sourceType, Stream input)
        {
            return await sellersImportAppService.ImportSellersAsync(sourceType, input);
        }
    }
}
