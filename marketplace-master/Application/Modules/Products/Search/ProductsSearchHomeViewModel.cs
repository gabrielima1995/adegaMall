using System;
using Domain.Entities;
using Domain.Entities.StoreModule;
using System.Collections.Generic;
using System.Globalization;

namespace Application.Modules.Products.Search
{
    /// <summary>
    /// Filtro para busca de produtos na página inicial.
    /// </summary>
    public class ProductsSearchHomeViewModel : IWithProductCategory, IWithCity
    {
        /// <summary>
        /// Filtro por parte do nome do produto.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// Filtro por categoria.
        /// </summary>
        public int? Category { get; set; }

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
        /// Filtro para distância.
        /// </summary>
        public long? LocationRange { get; set; }

        /// <summary>
        /// Idioma do usuário.
        /// </summary>
        public string Lang { get; set; }

        #region Disponibilidade do produto

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public SearchDaysOfWeek SearchDaysOfWeek { get; set; }

        public bool OnlyMorning { get; set; }
        public bool OnlyAfternoon { get; set; }
        public bool OnlyNight { get; set; }

        #endregion Disponibilidade do produto

        /// <summary>
        /// Lista de categorias dos produtos.
        /// </summary>
        public IList<ProductCategory> Categories { get; set; } = new List<ProductCategory>();

        public ProductsSearchHomeViewModel()
        {
            Lang = CultureInfo.CurrentCulture.GetCurrentLang();
        }

        public ProductsSearchHomeViewModel(IList<ProductCategory> categories) : this()
        {
            Categories = categories;
        }
    }
}