using Application.Config;
using Domain.DTO;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using V2.Application.Modules.SellerAssistantModule;

namespace Presentation.Helpers
{
    public static class SellerAssistantPresentation
    {
        /// <summary>
        /// Classe CSS para evidenciar links e botões que o assistente de vendedor não tem acesso.
        /// </summary>
        public static string DisabledClass => "action-disabled-for-assistant";

        public static string GetAllowedPageForAssistant(this LoginProfile profile, string preferred = null)
        {
            var pages = profile.GetAllowedPagesForAssistant();
            Uri uri;

            if (Uri.TryCreate(preferred, UriKind.Absolute, out uri) || Uri.TryCreate(new Uri(MaaSConfig.Current.Environment.AppUrl), preferred, out uri))
            {
                var path = uri.AbsolutePath;
                var match = pages.FirstOrDefault(page => path.StartsWith(page));
                if (match != null) return match;
            }

            return pages.FirstOrDefault() ?? "/";
        }

        public static string[] GetAllowedPagesForAssistant(this LoginProfile profile)
        {
            if (!profile.IsSellerAssistant()) return Array.Empty<string>();
            if (!profile.AssistantPermissions.Any()) return Array.Empty<string>();

            var pages = new List<string>();
            var config = MaaSConfig.Current;

            switch (profile.AssistantPermissions.First())
            {
                case SellerAssistantPermissions.DashboardEdit:
                case SellerAssistantPermissions.DashboardView:
                    if (config.Dashboard.SellerDashboard.Enabled)
                        pages.Add("/manage/dashboard");
                    break;

                case SellerAssistantPermissions.QuotesEdit:
                case SellerAssistantPermissions.QuotesView:
                case SellerAssistantPermissions.LeadsEdit:
                case SellerAssistantPermissions.LeadsView:
                    if (config.Quote.Enabled)
                        pages.Add("/quotes");
                    break;

                case SellerAssistantPermissions.ScheduleEdit:
                case SellerAssistantPermissions.ScheduleView:
                    if (config.Product.Enabled && config.Product.BusinessModel.Rental.Enabled)
                        pages.Add("/schedule");
                    break;

                case SellerAssistantPermissions.SalesEdit:
                case SellerAssistantPermissions.SalesView:
                    if (config.Product.Enabled && config.Payment.Enabled)
                        pages.Add("/mysales");
                    break;

                case SellerAssistantPermissions.PortfolioEdit:
                case SellerAssistantPermissions.PortfolioView:
                    if (config.Portfolio.Enabled)
                        pages.Add("/portfolio");
                    break;

                case SellerAssistantPermissions.ProductsEdit:
                case SellerAssistantPermissions.ProductsView:
                case SellerAssistantPermissions.StoreEdit:
                case SellerAssistantPermissions.StoreView:
                    if (config.Product.Enabled && !config.Product.HideStoreMenusFromSeller)
                        pages.Add("/store");
                    break;

                case SellerAssistantPermissions.ProfileEdit:
                case SellerAssistantPermissions.ProfileView:
                    pages.Add("/manage/seller");
                    break;

                case SellerAssistantPermissions.DisputesEdit:
                case SellerAssistantPermissions.DisputesView:
                    if (config.Product.Enabled && config.Payment.Enabled && config.Payment.Common.EnableTransactionWarranty)
                        pages.Add("/dispute");
                    break;

                case SellerAssistantPermissions.BalanceEdit:
                case SellerAssistantPermissions.BalanceView:
                    if (config.Product.Enabled && config.Payment.Enabled)
                        pages.Add("/manage/balance");
                    break;

                case SellerAssistantPermissions.GatewayEdit:
                case SellerAssistantPermissions.GatewayView:
                    if (config.Product.Enabled && config.Payment.Enabled)
                        pages.Add("/manage/gateway");
                    break;
            }

            return pages.ToArray();
        }
    }
}