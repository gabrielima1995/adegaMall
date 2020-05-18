using System.Collections.Generic;

namespace Application.Search
{
    /// <summary>
    /// Resultado de uma busca.
    /// </summary>
    /// <typeparam name="T">Tipo do resultado.</typeparam>
    public interface ISearchResult<T>
        where T : class
    {
        /// <summary>
        /// Lista de resultados da busca.
        /// </summary>
        IList<T> Results { get; }

        /// <summary>
        /// Url da próxima página de resultados.
        /// </summary>
        string NextResults { get; }
    }
}