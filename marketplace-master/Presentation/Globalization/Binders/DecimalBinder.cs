using Presentation.Globalization;
using System.Globalization;

namespace System.Web.Mvc
{
    public class DecimalBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };

            object actualValue = null;

            try
            {
                actualValue = Convert.ToDecimal(valueResult.AttemptedValue, CultureInfo.GetCultureInfo(controllerContext.HttpContext.Request.GetCurrentLang()));
            }
            catch (FormatException e)
            {
                modelState.Errors.Add(e);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

            return actualValue;
        }
    }
}