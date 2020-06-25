using Application.Config.Common;

namespace Application.Config.General.Layout
{
    public class PaymentMethodsDisplayConfig
    {
        public StatusConfig ShowInFooter { get; set; } = new StatusConfig(true);

        public StatusConfig ShowInProductDetailsScreen { get; set; } = new StatusConfig(false);

        public StatusConfig ShowInCartScreen { get; set; } = new StatusConfig(false);

        public StatusConfig ShowInCheckoutScreen { get; set; } = new StatusConfig(false);

        public StatusConfig ShowInSubscriptionScreen { get; set; } = new StatusConfig(false);
    }
}
