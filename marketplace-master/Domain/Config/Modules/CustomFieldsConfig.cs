using Application.Config.Common;
using Domain.Entities;

namespace Application.Config
{
    public class CustomFieldsConfig
    {
        public CustomFieldEntityConfig Seller { get; set; } = new CustomFieldEntityConfig();

        public CustomFieldEntityConfig Buyer { get; set; } = new CustomFieldEntityConfig();

        public CustomFieldEntityConfig Quotes { get; set; } = new CustomFieldEntityConfig();

        public CustomFieldEntityConfig Product { get; set; } = new CustomFieldEntityConfig();

        public CustomFieldEntityConfig Portfolio { get; set; } = new CustomFieldEntityConfig();

        public CustomFieldEntityConfig CartItem { get; set; } = new CustomFieldEntityConfig();

        public CustomFieldsConfig()
        {

        }

        public CustomFieldEntityConfig ForEntity(CustomFieldEntityType entityType)
        {
            switch (entityType)
            {
                case CustomFieldEntityType.Quote:
                    return Quotes;

                case CustomFieldEntityType.Buyer:
                    return Buyer;

                case CustomFieldEntityType.Seller:
                    return Seller;

                case CustomFieldEntityType.Product:
                    return Product;

                case CustomFieldEntityType.Portfolio:
                    return Portfolio;

                case CustomFieldEntityType.CartItem:
                    return CartItem;
            }

            return null;
        }
    }

    public class CustomFieldEntityConfig : StatusConfig
    {
        public int MaxCustomFields { get; set; } = 300;

        public CustomFieldEntityConfig() : base(false)
        {

        }
    }
}