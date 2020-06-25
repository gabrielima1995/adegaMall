using Application.Modules.BatchImport.Models;
using Domain.Modules.BatchImport;

namespace Presentation.Areas.Admin.Views.Products
{
    public class ImportResultsModel : ImportModelBase
    {
        public bool HasModelErrors { get; set; }

        public ImportResult<ProductData> Results { get; set; } = new ImportResult<ProductData>();
    }
}