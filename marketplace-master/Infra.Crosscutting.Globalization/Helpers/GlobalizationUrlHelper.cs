using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class GlobalizationUrlHelper
    {
        /// <summary>
        /// Obtem uma nova url com determinado idioma no parâmetro.
        /// </summary>
        /// <param name="url">Url.</param>
        /// <param name="lang">Idioma a ser inserido como parâmetro na nova url.</param>
        /// <returns>Nova url com o idioma no parâmetro.</returns>
        public static string GetNewUrlWithLang(this Uri url, string lang)
        {
            var pattern = @"[L,l]ang\=([\w]{2})[-]([\w]{2})";
            var builder = new UriBuilder(url);

            if (builder.Query.ContainsWithRegex(pattern))
                builder.Query = builder.Query.ReplaceWithRegex(pattern, "lang=" + lang).TrimStart('?');
            else
                builder.Query += (builder.Query.Length > 0 ? "&" : "") + "lang=" + lang;

            return builder.ToString();
        }
    }
}