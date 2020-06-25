using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace V2.Application.Modules.SellerAssistantModule
{
    public static class SellerAssistantPermissions
    {
        public const string ProfileEdit = "seller:profile:edit";
        public const string ProfileView = "seller:profile:view";

        public const string GatewayEdit = "seller:gateway:edit";
        public const string GatewayView = "seller:gateway:view";

        public const string StoreEdit = "seller:store:edit";
        public const string StoreView = "seller:store:view";

        public const string ProductsEdit = "seller:products:edit";
        public const string ProductsView = "seller:products:view";

        public const string ScheduleEdit = "seller:schedule:edit";
        public const string ScheduleView = "seller:schedule:view";

        public const string PortfolioEdit = "seller:portfolio:edit";
        public const string PortfolioView = "seller:portfolio:view";

        public const string QuotesEdit = "seller:quotes:edit";
        public const string QuotesView = "seller:quotes:view";

        public const string LeadsEdit = "seller:leads:edit";
        public const string LeadsView = "seller:leads:view";

        public const string SalesEdit = "seller:sales:edit";
        public const string SalesView = "seller:sales:view";

        public const string DisputesEdit = "seller:disputes:edit";
        public const string DisputesView = "seller:disputes:view";

        public const string DashboardEdit = "seller:dashboard:edit";
        public const string DashboardView = "seller:dashboard:view";

        public const string BalanceEdit = "seller:balance:edit";
        public const string BalanceView = "seller:balance:view";

        public static ReadOnlyCollection<string> AllPermissions => new List<string>
        {
            ProfileEdit,
            ProfileView,

            GatewayEdit,
            GatewayView,

            StoreEdit,
            StoreView,

            ProductsEdit,
            ProductsView,

            ScheduleEdit,
            ScheduleView,

            PortfolioEdit,
            PortfolioView,

            QuotesEdit,
            QuotesView,

            LeadsEdit,
            LeadsView,

            SalesEdit,
            SalesView,

            DisputesEdit,
            DisputesView,

            DashboardEdit,
            DashboardView,

            BalanceEdit,
            BalanceView

        }.AsReadOnly();
    }
}
