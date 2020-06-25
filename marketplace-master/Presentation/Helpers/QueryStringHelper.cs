using Flurl;
using System;

namespace Presentation.Helpers
{
    public static class QueryStringHelper
    {
        public static string AddOrUpdateValue(Uri baseUri, string name, string value)
        {
            return baseUri.PathAndQuery.SetQueryParam(name, value).ToString();

            //var path = baseUri.AbsolutePath;
            //var query = baseUri.Query;
            //if (query.Contains($"{name}=", StringComparison.OrdinalIgnoreCase))
            //{
            //    query = Regex.Replace(query, $"{name}={queryString[name]}", $"{name}={value}", RegexOptions.IgnoreCase);
            //    //query = query.Replace($"{name}={queryString["name"]}", $"{name}={value}");
            //}
            //else
            //{
            //    query = query + $"{name}={value}";
            //}
            //return path + query;
            ////return HttpUtility.UrlEncode(path + + query);
        }
    }
}