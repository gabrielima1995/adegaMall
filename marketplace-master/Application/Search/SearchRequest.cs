using Domain.Entities;
using System;

namespace Application.Search
{
    /// <summary>
    /// Parâmetros padrão para busca de resultados.
    /// </summary>
    public abstract class SearchRequest : IWithLang
    {
        private int limit;
        private int page;

        protected SearchRequest()
        {
            Limit = GetLimitDefault();
            Page = 1;
        }

        /// <summary>
        /// Idioma do usuário.
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Número de resultados por página.
        /// </summary>
        public int Limit
        {
            get => limit;
            set => limit = value > 0 ? value : GetLimitDefault();
        }

        /// <summary>
        /// Número da página dos resultados.
        /// </summary>
        public int Page
        {
            get => page;
            set => page = Math.Max(1, value);
        }

        /// <summary>
        /// Obtem o número de resultados por página.
        /// </summary>
        /// <returns></returns>
        protected abstract int GetLimitDefault();

        /// <summary>
        /// Obtem os parâmetros da próxima página de resultados.
        /// </summary>
        /// <returns></returns>
        public abstract string GetNextUrlParams();
    }
}