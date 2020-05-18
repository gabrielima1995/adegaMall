using System.Globalization;

namespace System.Web.Mvc
{
    public class NullableDateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            return value?.ConvertTo(typeof(DateTime), CultureInfo.GetCultureInfo(controllerContext.HttpContext.Request.GetCurrentLang()));
        }
    }
}