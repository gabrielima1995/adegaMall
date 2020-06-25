using Markdig;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        private static readonly Lazy<MarkdownPipeline> markdownPipeline = new Lazy<MarkdownPipeline>(() => new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

        public static IHtmlString TextWithLineBreaks(this HtmlHelper html, string content)
        {
            var str = (content ?? "").Replace("\n", "<br/>");
            return html.Raw(str);
        }

        public static IHtmlString Markdown(this HtmlHelper html, string content)
        {
            var processed = Markdig.Markdown.ToHtml(content, markdownPipeline.Value);
            return html.Raw(processed);
        }

        public static MvcHtmlString RadioButtonX(this HtmlHelper html, string name, object value, object currentValue, object htmlAttributes = null)
        {
            var isChecked = value != null && value.Equals(currentValue);
            return html.RadioButton(name, value, isChecked, htmlAttributes);
        }

        public static string RequireScript(this HtmlHelper html, string path, int priority = 1)
        {
            var requiredScripts = HttpContext.Current.Items["RequiredScripts"] as List<ResourceInclude>;
            if (requiredScripts == null) HttpContext.Current.Items["RequiredScripts"] = requiredScripts = new List<ResourceInclude>();
            if (!requiredScripts.Any(i => i.Path == path)) requiredScripts.Add(new ResourceInclude() { Path = path, Priority = priority });
            return null;
        }

        public static HtmlString EmitRequiredScripts(this HtmlHelper html)
        {
            if (!(HttpContext.Current.Items["RequiredScripts"] is List<ResourceInclude> requiredScripts))
                return null;

            var sb = new StringBuilder();
            foreach (var item in requiredScripts.OrderByDescending(i => i.Priority))
                sb.AppendFormat("<script src=\"{0}\" type=\"text/javascript\"></script>\n", item.Path);

            return new HtmlString(sb.ToString());
        }
    }

    public class ResourceInclude
    {
        public string Path { get; set; }
        public int Priority { get; set; }
    }
}