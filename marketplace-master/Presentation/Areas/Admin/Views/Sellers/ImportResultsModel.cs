using Application.Modules.BatchImport.Models;
using Application.Modules.BatchImport.Sellers;

namespace Presentation.Areas.Admin.Views.Sellers
{
    public class ImportResultsModel : ImportModelBase
    {
        public bool HasModelErrors { get; set; }

        public ImportResult<SellerDto> Results { get; set; } = new ImportResult<SellerDto>();
    }
}