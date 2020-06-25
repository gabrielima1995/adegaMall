using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Globalization
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Localized(this HtmlHelper htmlHelper, string labelName, string tagName = "span", object htmlAttributes = null)
        {
            return Localized(htmlHelper, labelName, tagName, htmlAttributes, null);
        }

        public static MvcHtmlString Localized(this HtmlHelper htmlHelper, string labelName, string tagName = "span", object htmlAttributes = null, params object[] formatArgs)
        {
            var text = HttpContext.Current.Request.GetLocalizedString(labelName, formatArgs);
            var tag = new TagBuilder(tagName);

            tag.Attributes.Add("data-label", labelName);
            tag.InnerHtml = text;

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tag.MergeAttributes(attributes);
            }

            return new MvcHtmlString(tag.ToString());
        }

        public static MvcHtmlString LocalizedString(this HtmlHelper htmlHelper, string labelName, params object[] formatArgs)
        {
            return new MvcHtmlString(HttpContext.Current.Request.GetLocalizedString(labelName, formatArgs));
        }

        public static MvcHtmlString Localized(this HtmlHelper htmlHelper, Enum enumValue, string tagName = "span", object htmlAttributes = null)
        {
            var text = GlobalizationEnumHelper.GetDescriptionLocalized(enumValue);
            var tag = new TagBuilder(tagName)
            {
                InnerHtml = text
            };

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tag.MergeAttributes(attributes);
            }

            return new MvcHtmlString(tag.ToString());
        }

        public static MvcHtmlString LocalizedString(this HtmlHelper htmlHelper, Enum enumValue)
        {
            return new MvcHtmlString(GlobalizationEnumHelper.GetDescriptionLocalized(enumValue));
        }
    }
}