using Application.Config;
using Domain.Entities;
using Domain.Interfaces;
using Framework;
using MediatR;
using Presentation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using V2.Application.Modules.SellerAssistantModule;
using V2.Application.Modules.UserAccounts;

namespace Presentation.Controllers
{
    [AuthorizeSeller]
    public class ManageAssistantController : BaseController
    {
        private readonly MaaSConfig config = MaaSConfig.Current;
        private readonly IMediator mediator;
        private readonly IUserAuthProvider userAuth;

        public ManageAssistantController(IMediator mediator, IUserAuthProvider userAuth)
        {
            this.mediator = mediator;
            this.userAuth = userAuth;
        }

        public async Task<ActionResult> Index()
        {
            if (!config.Seller.Assistant.Enabled)
                return HttpNotFound();

            var data = await GetIndexViewModelAsync();
            if (data == null)
                return RedirectToAction("Setup");

            return View(BuildViewModel(data));
        }

        [HttpPost]
        public async Task<ActionResult> Index(ViewModelBase<ManageAssistantIndexViewModel> model)
        {
            var permissions = new List<string>();
            foreach (var item in model.Data.Permissions)
            {
                if (item.CanView) permissions.Add(item.ViewPermissionName);
                if (item.CanEdit) permissions.Add(item.EditPermissionName);
            }

            var sellerId = userAuth.PersonId;
            await mediator.Send(new SetSellerAssistantPermissions(sellerId, permissions));

            this.SetSuccessMessage(true);
            return View(model);
        }

        public async Task<ActionResult> Setup()
        {
            if (!config.Seller.Assistant.Enabled)
                return HttpNotFound();

            var model = await GetIndexViewModelAsync();
            if (model != null)
                return RedirectToAction("Index");

            var data = new ManageAssistantSetupViewModel();
            return View(BuildViewModel(data));
        }

        [HttpPost]
        public async Task<ActionResult> Setup(ViewModelBase<ManageAssistantSetupViewModel> model)
        {
            if (!config.Seller.Assistant.Enabled)
                return HttpNotFound();

            if (ModelState.IsValid)
            {
                var sellerId = userAuth.PersonId;
                var userAndProfile = SellerAssistantUserAndProfile.Create(UserOrPersonId.Generate(), model.Data.Email, sellerId);

                var registration = await mediator.Send(new RegisterSellerAssistantUserAndProfile(userAndProfile));
                if (registration.IsFailure)
                {
                    ModelState.AddErrors(registration);
                    return View(model);
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Enable()
        {
            if (!config.Seller.Assistant.Enabled)
                return HttpNotFound();
            await SetSellerAssistantState(true);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Disable()
        {
            if (!config.Seller.Assistant.Enabled)
                return HttpNotFound();
            await SetSellerAssistantState(false);
            return RedirectToAction("Index");
        }

        private async Task SetSellerAssistantState(bool enabled)
        {
            var sellerId = userAuth.PersonId;
            await mediator.Send(new EnableOrDisableSellerAssistant(sellerId, enabled));
        }

        private async Task<ManageAssistantIndexViewModel> GetIndexViewModelAsync()
        {
            var sellerId = userAuth.PersonId;
            var assistant = await mediator.Send(new GetSellerAssistant(sellerId));

            if (assistant == null)
                return null;

            var permissions = new List<ManageAssistantPermissionViewModel>
            {
                ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_PROFILE",
                    SellerAssistantPermissions.ProfileEdit,
                    SellerAssistantPermissions.ProfileView,
                    assistant.Permissions)
            };

            if (config.Payment.Enabled && (config.Product.Enabled || config.Quote.QuoteBilling.Enabled))
            {
                permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_GATEWAY",
                    SellerAssistantPermissions.GatewayEdit,
                    SellerAssistantPermissions.GatewayView,
                    assistant.Permissions));

                permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_SALES",
                    SellerAssistantPermissions.SalesEdit,
                    SellerAssistantPermissions.SalesView,
                    assistant.Permissions));

                permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_BALANCE",
                    SellerAssistantPermissions.BalanceEdit,
                    SellerAssistantPermissions.BalanceView,
                    assistant.Permissions));

                if (config.Payment.Common.EnableTransactionWarranty)
                {
                    permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_DISPUTES",
                    SellerAssistantPermissions.DisputesEdit,
                    SellerAssistantPermissions.DisputesView,
                    assistant.Permissions));
                }
            }

            if (config.Product.Enabled && !config.Product.HideStoreMenusFromSeller)
            {
                permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_STORE",
                    SellerAssistantPermissions.StoreEdit,
                    SellerAssistantPermissions.StoreView,
                    assistant.Permissions));

                permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_PRODUCTS",
                    SellerAssistantPermissions.ProductsEdit,
                    SellerAssistantPermissions.ProductsView,
                    assistant.Permissions));
            }

            if (config.Product.Enabled && config.Product.BusinessModel.Rental.Enabled)
            {
                permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_SCHEDULE",
                    SellerAssistantPermissions.ScheduleEdit,
                    SellerAssistantPermissions.ScheduleView,
                    assistant.Permissions));
            }

            if (config.Portfolio.Enabled)
            {
                permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_PORTFOLIO",
                    SellerAssistantPermissions.PortfolioEdit,
                    SellerAssistantPermissions.PortfolioView,
                    assistant.Permissions));
            }

            if (config.Quote.Enabled)
            {
                permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_QUOTES",
                    SellerAssistantPermissions.QuotesEdit,
                    SellerAssistantPermissions.QuotesView,
                    assistant.Permissions));

                if (config.Quote.LeadSales.Enabled)
                {
                    permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_LEADS",
                        SellerAssistantPermissions.LeadsEdit,
                        SellerAssistantPermissions.LeadsView,
                        assistant.Permissions));
                }
            }

            if (config.Dashboard.SellerDashboard.Enabled)
            {
                permissions.Add(ManageAssistantPermissionViewModel.Create("SELLER_ASSISTANT_PERMISSIONS_DASHBOARD",
                    SellerAssistantPermissions.DashboardEdit,
                    SellerAssistantPermissions.DashboardView,
                    assistant.Permissions));
            }

            return new ManageAssistantIndexViewModel
            {
                Email = assistant.Email,
                IsEnabled = assistant.IsEnabled,
                Permissions = permissions
            };
        }
    }

    public class ManageAssistantIndexViewModel
    {
        public string Email { get; set; }
        public bool IsEnabled { get; set; }
        public List<ManageAssistantPermissionViewModel> Permissions { get; set; }
    }

    public class ManageAssistantPermissionViewModel
    {
        public string SubjectLabel { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public string ViewPermissionName { get; set; }
        public string EditPermissionName { get; set; }

        public static ManageAssistantPermissionViewModel Create(string label, string editPermissionName, string viewPermissionName, IEnumerable<string> userPermissions)
        {
            return new ManageAssistantPermissionViewModel
            {
                SubjectLabel = label,
                CanView = userPermissions.Contains(viewPermissionName),
                CanEdit = userPermissions.Contains(editPermissionName),
                ViewPermissionName = viewPermissionName,
                EditPermissionName = editPermissionName
            };
        }
    }

    public class ManageAssistantSetupViewModel
    {
        [MaxLength(320)]
        [RequiredLocalized]
        [EmailAddressLocalized]
        public string Email { get; set; }
    }
}