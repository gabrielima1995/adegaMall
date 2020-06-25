using Domain.Entities;
using Domain.Entities.StoreModule;
using System;
using System.Collections.Generic;

namespace Application.Modules.Products.Search
{
    /// <summary>
    /// Dados da busca de produtos.
    /// </summary>
    public class SearchViewModel : SearchResultViewModel, IWithProductCategory, IWithLang, IWithCity
    {
        /// <summary>
        /// Filtro por parte do nome do produto.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Filtro por cidade (produtos por retirada no local)
        /// </summary>
        public string CityAndState { get; set; }

        /// <summary>
        /// Filtro por latitude da região de entrega.
        /// </summary>
        public double? Lat { get; set; }

        /// <summary>
        /// Filtro por longitude da região de entrega.
        /// </summary>
        public double? Lng { get; set; }

        /// <summary>
        /// Filtro por categoria.
        /// </summary>
        public int? Category { get; set; }

        /// <summary>
        /// Filtro por somente retirada no local.
        /// </summary>
        public bool OnlyOnsitePickup { get; set; }

        /// <summary>
        /// Filtro por somente entrega física.
        /// </summary>
        public bool OnlyShippingOut { get; set; }

        /// <summary>
        /// Filtro por distância.
        /// </summary>
        public long? LocationRange { get; set; }

        /// <summary>
        /// Idioma do usuário.
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Lista de categorias de produtos.
        /// </summary>
        public IList<ProductCategory> Categories { get; set; }

        /// <summary>
        /// O vendedor pelo qual a busca foi filtrada (caso tenha sido).
        /// </summary>
        public Seller FilteredBySeller { get; set; }

        /// <summary>
        /// Total de itens que podem ser filtrados.
        /// </summary>
        public int Total { get; set; }

        #region Disponibilidade do produto

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public SearchDaysOfWeek SearchDaysOfWeek { get; set; }

        public bool OnlyMorning { get; set; }
        public bool OnlyAfternoon { get; set; }
        public bool OnlyNight { get; set; }

        #endregion Disponibilidade do produto

        public SearchViewModel(SearchRequestViewModel request)
        {
            Product = request.Product;
            CityAndState = request.CityAndState;
            Lat = request.Lat;
            Lng = request.Lng;
            Category = request.Category;
            OnlyOnsitePickup = request.OnlyOnsitePickup;
            OnlyShippingOut = request.OnlyShippingOut;
            LocationRange = request.LocationRange;
            Lang = request.Lang;
            StartDate = request.StartDate;
            EndDate = request.EndDate;
            SearchDaysOfWeek = request.SearchDaysOfWeek;
            OnlyMorning = request.OnlyMorning;
            OnlyAfternoon = request.OnlyAfternoon;
            OnlyNight = request.OnlyNight;
        }
    }
}