using Application.Modules.BatchImport.Models;

namespace Presentation.Areas.Admin.Views.Products
{
    public class ImportImagesResultsModel : ImportModelBase
    {
        public ImportImagesResultsModel(ImportResult<string> results)
        {
            Results = results ?? throw new System.ArgumentNullException(nameof(results));
        }

        public ImportResult<string> Results { get; }
    }
}