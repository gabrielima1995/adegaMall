using Presentation.Areas.Admin.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Presentation.Areas.Admin.Views.Sellers
{
    public class ImportModel : ImportModelBase { }

    public abstract class ImportModelBase : BaseAdminModel
    {
        public bool SendWelcomeEmailToImportedSellers { get; set; } = true;

        public string SourceType { get; set; }

        public ImportSourceTypeList SourceTypes => new ImportSourceTypeList(SourceType);
    }

    public class ImportSourceTypeList : SelectList
    {
        private static IDictionary<string, string> Options => new Dictionary<string, string>
        {
            { "excel", "Planilha do Excel" },
            { "csv", "Arquivo CSV" }
        };

        public ImportSourceTypeList(string selected = null) : base(Options, "Key", "Value", selected) { }
    }
}