using Application.Config.Common;
using Application.Config.Modules.Product;
using Domain.Config.Modules.Product;
using Domain.Entities.StoreModule;
using FluentValidation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Application.Config.Modules
{
    /// <summary>
    /// Configuração do módulo de produtos.
    /// </summary>
    public class ProductConfig : StatusConfig
    {
        public BusinessModelConfig BusinessModel { get; set; } = new BusinessModelConfig();

        public int ProductsLimit { get; set; }

        public bool EnableInstallmentsPerProduct { get; set; } = false;

        public bool CanPauseProducts { get; set; } = false;

        public bool ShowRelatedProducts { get; set; } = false;

        /// <summary>
        /// Oculta a navegação de loja e produtos do Minha Conta do Vendedor.
        /// </summary>
        public bool HideStoreMenusFromSeller { get; set; } = false;

        public RegisterConfig Image { get; set; } = new RegisterConfig();

        public RegisterConfig Specifications { get; set; } = new RegisterConfig();

        public RegisterConfig CustomId { get; set; } = new RegisterConfig();

        public RegisterConfig Category { get; set; } = new RegisterConfig();

        public RegisterConfig Price { get; set; } = new RegisterConfig();

        public StatusConfig DiscountPrice { get; set; } = new StatusConfig(true);

        public RegisterConfig Length { get; set; } = new RegisterConfig();

        public RegisterConfig Width { get; set; } = new RegisterConfig();

        public RegisterConfig Height { get; set; } = new RegisterConfig();

        public RegisterConfig Weight { get; set; } = new RegisterConfig();

        [JsonIgnore]
        public bool AnyDimensionsEnabled => Length.Enabled || Width.Enabled || Height.Enabled || Weight.Enabled;

        /// <summary>
        /// Configuração da página de detalhes do produto.
        /// </summary>
        public ProductPageConfig Page { get; set; } = new ProductPageConfig();

        /// <summary>
        /// Configuração de variantes de produto.
        /// </summary>
        public VariantsConfig Variants { get; set; } = new VariantsConfig();

        /// <summary>
        /// Configuração de busca no módulo de produtos.
        /// </summary>
        public ProductSearchConfig Search { get; set; } = new ProductSearchConfig();

        /// <summary>
        /// Configuração de avaliação de pedido
        /// </summary>
        public OrderReviewConfig OrderReview { get; set; } = new OrderReviewConfig();

        /// <summary>
        /// Configurações gerais de pedidos.
        /// </summary>
        public OrderConfig Order { get; set; } = new OrderConfig();

        /// <summary>
        /// Configurações gerais de Estoque.
        /// </summary>
        public StockConfig Stock { get; set; } = new StockConfig();

        #region Shipping

        /// <summary>
        /// Configuração de modalidade de operação de frete suportada pela plataforma.
        /// </summary>
        public ShippingMode ShippingMode { get; set; }

        /// <summary>
        /// Configuração de entrega de frota própria/privada.
        /// </summary>
        public PrivateFleetShippingConfig PrivateFleetShipping { get; set; } = new PrivateFleetShippingConfig();

        /// <summary>
        /// Configuração de retirada no local.
        /// </summary>
        public OnSitePickupShippingConfig OnSitePickupShipping { get; set; } = new OnSitePickupShippingConfig();

        /// <summary>
        /// Configuração de entrega de produto digital/download (sem entrega física).
        /// </summary>
        public NoShippingConfig NoShipping { get; set; } = new NoShippingConfig();

        /// <summary>
        /// Configuração de entrega de produto sem entrega (ex. serviços, itens entregues por fora da plataforma).
        /// </summary>
        public ActualNoShippingConfig ActualNoShipping { get; set; } = new ActualNoShippingConfig();

        /// <summary>
        /// Configuração de entrega Correios Sedex.
        /// </summary>
        public PostalExpressShippingConfig SedexShipping { get; set; } = new PostalExpressShippingConfig();

        /// <summary>
        /// Configuração de entrega Correios PAC.
        /// </summary>
        public PostalRegularShippingConfig PACShipping { get; set; } = new PostalRegularShippingConfig();

        /// <summary>
        /// Configuração de entrega AsapLog.
        /// </summary>
        public AsapLogShippingConfig AsapLogShipping { get; set; } = new AsapLogShippingConfig();

        public int CorreiosTimeoutSeconds { get; set; } = 6;

        /// <summary>
        /// Define os dados de contrato com os Correios, para uso no WebService de cálculo de preço e prazo.
        /// </summary>
        public CorreiosContractConfig CorreiosContract { get; set; } = new CorreiosContractConfig();

        /// <summary>
        /// Indica se pelo menos um método de entrega está habilitado no módulo.
        /// </summary>
        [JsonIgnore]
        public bool SomeShippingMethodEnabled => GetShippingMethodsEnabled().Count > 0;

        [JsonIgnore]
        public bool HasPhysicalShipping => GetShippingMethodsEnabled().Intersect(new[] { ShippingMethod.PostalExpress, ShippingMethod.PostalRegular, ShippingMethod.PrivateFleet, ShippingMethod.AsapLog }).Any();

        /// <summary>
        /// Indica se somente um método de entrega está habilitado no módulo.
        /// </summary>
        [JsonIgnore]
        public bool OnlyOneShippingMethodEnabled => GetShippingMethodsEnabled().Count == 1;

        /// <summary>
        /// Prioriza Sedex no checkout se habilitado
        /// </summary>
        public bool PrioritizeExpressShipping { get; set; } = false;

        /// <summary>
        /// Obtem os métodos de entrega habilitados no módulo.
        /// </summary>
        public IList<ShippingMethod> GetShippingMethodsEnabled()
        {
            var list = new List<ShippingMethod>();

            if (ShippingMode == ShippingMode.PerProduct)
            {
                if (PrivateFleetShipping.Enabled)
                    list.Add(ShippingMethod.PrivateFleet);

                if (OnSitePickupShipping.Enabled)
                    list.Add(ShippingMethod.OnSitePickup);

                if (NoShipping.Enabled)
                    list.Add(ShippingMethod.NoShipping);

                if (ActualNoShipping.Enabled)
                    list.Add(ShippingMethod.ActualNoShipping);

                if (SedexShipping.Enabled)
                    list.Add(ShippingMethod.PostalExpress);

                if (PACShipping.Enabled)
                    list.Add(ShippingMethod.PostalRegular);

                if (AsapLogShipping.Enabled)
                    list.Add(ShippingMethod.AsapLog);
            }
            else if (ShippingMode == ShippingMode.PerStore)
            {
                if (PrivateFleetShipping.Enabled)
                    list.Add(ShippingMethod.PrivateFleet);

                if (OnSitePickupShipping.Enabled)
                    list.Add(ShippingMethod.OnSitePickup);

                if (SedexShipping.Enabled)
                    list.Add(ShippingMethod.PostalExpress);

                if (PACShipping.Enabled)
                    list.Add(ShippingMethod.PostalRegular);

                if (AsapLogShipping.Enabled)
                    list.Add(ShippingMethod.AsapLog);
            }

            return list;
        }

        #endregion Shipping

        /// <summary>
        /// Configurações de lista de desejos (produtos favoritados).
        /// </summary>
        public StatusConfig Wishlist { get; set; } = new StatusConfig(false);

        /// <summary>
        /// Configurações para modelos de produtos.
        /// </summary>
        public ProductTemplatesConfig Templates { get; set; } = new ProductTemplatesConfig();

        public ProductConfig() : base(enabled: false)
        {
        }
    }

    public class ProductConfigValidator : AbstractValidator<ProductConfig>
    {
        public ProductConfigValidator()
        {
            RuleFor(x => x.BusinessModel).SetValidator(new BusinessModelConfigValidator());
        }
    }
}