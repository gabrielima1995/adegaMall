using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Framework
{
    /// <summary>
    /// Representa os dados de uma página de resultados.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingResult<T>
    {
        /// <summary>
        /// Resultados da página atual.
        /// </summary>
        public IList<T> Data { get; private set; }

        /// <summary>
        /// Total de itens para o filtro.
        /// </summary>
        public int TotalItems { get; private set; }

        /// <summary>
        /// Url da próxima página de resultados.
        /// </summary>
        public string NextPage { get; private set; }

        /// <summary>
        /// Url da página anterior de resultados.
        /// </summary>
        public string PrevPage { get; private set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data">Tupla com os dados e total de itens</param>
        /// <param name="start">Número inteiro, indica o ponto de início dos resultados (quantos registros serão omitidos do início da listagem)</param>
        /// <param name="limit">Número inteiro, determina o máximo de registros que serão retornados</param>
        /// <param name="resourceUrl">Url do recurso (com todos os parametros de filtro, se houver)</param>
        public PagingResult(Tuple<IList<T>, int> data, int start, int limit, string resourceUrl)
        {
            Data = data.Item1;
            TotalItems = data.Item2;

            var nextStart = start + limit;

            // Se tem próxima página de itens
            if (nextStart < TotalItems)
            {
                NextPage = ConfigUrl(resourceUrl, nextStart, limit);
            }

            // Se tem página anterior de itens e os filtros retornam resultados.
            if (start > 0 && TotalItems > 0)
            {
                var prevStart = start - limit;

                // Previne prev start negativo
                prevStart = Math.Max(prevStart, 0);

                // Previne prev start maior que total de itens
                prevStart = Math.Min(prevStart, TotalItems - 1);

                PrevPage = ConfigUrl(resourceUrl, prevStart, limit);
            }
        }

        private string ConfigUrl(string resourceUrl, int newStart, int limit)
        {
            var uri = new UriBuilder(new Uri(resourceUrl));
            var uriQs = HttpUtility.ParseQueryString(uri.Query);
            uriQs["start"] = newStart.ToString();
            uriQs["limit"] = limit.ToString();

            uri.Query = string.Join("&", uriQs.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(uriQs[a])));

            return uri.ToString();
        }
    }

    public static class PagingResultExtensions
    {
        /// <summary>
        /// Converte para objeto de uma página de resultados.
        /// </summary>
        /// <param name="data">Tupla com os dados e total de itens.</param>
        /// <param name="start">Número inteiro, indica o ponto de início dos resultados (quantos registros serão omitidos do início da listagem).</param>
        /// <param name="limit">Número inteiro, determina o máximo de registros que serão retornados.</param>
        /// <param name="resourceUrl">Url do recurso (com todos os parametros de filtro, se houver).</param>
        public static PagingResult<T> ToPagingResult<T>(this Tuple<IList<T>, int> data, int start, int limit, string resourceUrl)
        {
            return new PagingResult<T>(data, start, limit, resourceUrl);
        }
    }
}