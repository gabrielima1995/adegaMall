using Application.Modules.Products.Search;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Presentation.Helpers.CustomModelBinders
{
    public class ProductSearchRequestModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = (SearchRequestViewModel)base.BindModel(controllerContext, bindingContext);

            if (model != null)
            {
                try
                {
                    var qs = controllerContext.HttpContext.Request.QueryString["f"];
                    model.VariantIds = qs != null ? qs.Split(',').Select(int.Parse).ToList() : new List<int>();
                }
                catch { }
            }

            return model;
        }
    }
}