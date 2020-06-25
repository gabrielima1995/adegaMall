using Application.Config.General.Layout;
using System.Collections.Generic;

namespace Application.Config.General
{
    public class LayoutConfig
    {
        public FontsConfig Fonts { get; set; } = new FontsConfig();

        public ImagesConfig Images { get; set; } = new ImagesConfig();

        public StylesConfig Styles { get; set; } = new StylesConfig();

        public ColorsConfig Colors { get; set; } = new ColorsConfig();

        public PaymentMethodsDisplayConfig PaymentMethodsDisplay { get; set; } = new PaymentMethodsDisplayConfig() { ShowInCartScreen = false, ShowInCheckoutScreen = true, ShowInFooter = true, ShowInSubscriptionScreen = true };

        public TransactionWarrantyDisplayConfig TransactionWarrantyDisplay { get; set; } = new TransactionWarrantyDisplayConfig();

        public ExtendedDictionary<string, string> CreateSassVariables()
        {
            return new ExtendedDictionary<string, string>().Extend(Fonts, Images.CreateSassVariables(), Styles, Colors);
        }
    }
}