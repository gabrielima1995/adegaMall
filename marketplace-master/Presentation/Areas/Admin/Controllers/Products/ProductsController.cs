using Application.Config;
using Application.Modules.BatchImport.Models;
using Application.Modules.BatchImport.Products;
using Presentation.Areas.Admin.Views.Products;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Controllers.Products
{
    [AuthorizeAdmin]
    public class ProductsController : Controller
    {
        private readonly ProductsImportAppService productsImportAppService;

        public ProductsController(ProductsImportAppService productsImportAppService)
        {
            this.productsImportAppService = productsImportAppService;
        }

        public async Task<ActionResult> DownloadImportTemplate(string type)
        {
            var sourceType = ParseSourceType(type);
            var command = new GenerateProductTemplateCommand(sourceType);
            var stream = await productsImportAppService.GenerateProductTemplateAsync(command);

            switch (sourceType)
            {
                case BatchImportSourceType.Excel:
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "template.xlsx");

                case BatchImportSourceType.Csv:
                    return File(stream, "text/csv", "template.csv");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ActionResult Import()
        {
            if (!MaaSConfig.Current.BatchImport.Products.Enabled)
                return new HttpNotFoundResult();

            return View(new ImportModel());
        }

        public ActionResult ImportResults()
        {
            return RedirectToAction("Import");
        }

        [HttpPost]
        public async Task<ActionResult> ImportResults(HttpPostedFileBase file)
        {
            if (!MaaSConfig.Current.BatchImport.Products.Enabled)
                return new HttpNotFoundResult();

            var sourceType = ParseSourceType(Request.Form["SourceType"]);
            var command = new ImportProductsCommand(sourceType, file.InputStream);
            var results = await productsImportAppService.ImportProductsAsync(command);

            return View(new ImportResultsModel
            {
                SourceType = Request.Form["SourceType"],
                Results = results
            });
        }

        public async Task<ActionResult> ImportImagesResults(HttpPostedFileBase file)
        {
            if (!MaaSConfig.Current.BatchImport.Products.Enabled)
                return new HttpNotFoundResult();

            var command = new ImportProductImagesCommand(file.InputStream);
            var results = await productsImportAppService.ImportProductImagesAsync(command);

            return View(new ImportImagesResultsModel(results));
        }

        private BatchImportSourceType ParseSourceType(string type) => (BatchImportSourceType)
            Enum.Parse(typeof(BatchImportSourceType), type, ignoreCase: true);
    }
}