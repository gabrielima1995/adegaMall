using System.Collections.Generic;

namespace System.Web.Mvc
{
    public static class ViewHelper
    {
        private static string GetMessageData(IDictionary<string, object> dict, string key)
        {
            return dict.ContainsKey(key) ? dict[key].ToString() : null;
        }

        private static string GetMessageData(WebViewPage view, string key)
        {
            var data = GetMessageData(view.ViewData, key);

            return !string.IsNullOrEmpty(data)
                ? data
                : GetMessageData(view.TempData, key);
        }

        public static string GetSuccessMessage(this WebViewPage view)
        {
            return GetMessageData(view, MessageKeys.SUCCESS);
        }

        public static string GetWarningMessage(this WebViewPage view)
        {
            return GetMessageData(view, MessageKeys.WARNING);
        }

        public static string GetErrorMessage(this WebViewPage view)
        {
            return GetMessageData(view, MessageKeys.ERROR);
        }
    }
}