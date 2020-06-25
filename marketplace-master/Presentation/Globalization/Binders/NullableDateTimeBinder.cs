using Application.Config;
using Presentation.Globalization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace System.Web.Mvc
{
    public class NullableDateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = GetAttemptedValue(bindingContext);
            if (value == null || string.IsNullOrWhiteSpace(value.AttemptedValue)) return null;

            foreach (var culture in GetCultures(controllerContext))
                if (TryConvert(value, culture, out DateTime converted))
                    return converted;

            return null;
        }

        private ValueProviderResult GetAttemptedValue(ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);
            return value;
        }

        private bool TryConvert(ValueProviderResult value, CultureInfo culture, out DateTime converted)
        {
            try
            {
                converted = (DateTime)value.ConvertTo(typeof(DateTime), culture);
                return true;
            }
            catch
            {
                converted = DateTime.MinValue;
                return false;
            }
        }

        private IEnumerable<CultureInfo> GetCultures(ControllerContext controllerContext)
        {
            var cultures = new List<CultureInfo>();
            cultures.AddRange(GetBrowserLanguages(controllerContext));
            cultures.AddRange(GetPlatformLanguages(controllerContext));
            return cultures;
        }

        private IEnumerable<CultureInfo> GetBrowserLanguages(ControllerContext context)
        {
            var userLanguages = context?.HttpContext?.Request?.UserLanguages;

            if (userLanguages == null)
                return new[] { CultureInfo.GetCultureInfo(MaaSConfig.Current.Localization.DefaultLang) };

            var cultures = new List<CultureInfo>();
            foreach (var language in userLanguages)
            {
                var cultureName = language.Split(new[] { ',', ';' }).First();
                if (TryCreateSpecificCulture(cultureName, out CultureInfo cultureInfo))
                    cultures.Add(cultureInfo);
            }
            
            return cultures;
        }

        private IEnumerable<CultureInfo> GetPlatformLanguages(ControllerContext context)
        {
            var requestLang = context?.HttpContext?.Request?.GetCurrentLang();

            if (requestLang == null)
                return Array.Empty<CultureInfo>();

            return new[] { CultureInfo.GetCultureInfo(requestLang) };
        }

        private bool TryCreateSpecificCulture(string cultureName, out CultureInfo cultureInfo)
        {
            try
            {
                cultureInfo = CultureInfo.CreateSpecificCulture(cultureName);
                return true;
            }
            catch
            {
                cultureInfo = CultureInfo.InvariantCulture;
                return false;
            }
        }
    }
}