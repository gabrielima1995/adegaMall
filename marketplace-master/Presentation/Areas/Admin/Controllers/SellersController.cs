using Application.Config;
using Application.Modules.BatchImport;
using Application.Modules.BatchImport.Models;
using Application.Modules.Ratings;
using Application.Sellers;
using Domain.DTO;
using Domain.Interfaces.Repositories;
using Domain.Modules.Ratings.Queries;
using Domain.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NewContext.Core;
using NewContext.Queries.Sellers;
using Presentation.Areas.Admin.Views.Sellers;
using Presentation.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.LeadSales;
using V2.Application.Modules.Sellers;
using V2.Application.Modules.StoreModule;

namespace Presentation.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class SellersController : Controller
    {
        private readonly BatchImportAppService batchImportAppService;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly ILeadSalesAppService leadSalesAppService;
        private readonly IProductRepository productRepository;
        private readonly IUserRatingAppService ratingAppService;
        private readonly IRatingsQueries ratingsQueries;
        private readonly SellerAppService sellerAppService;
        private readonly ISellerRepository sellerRepository;
        private readonly ISellerScheduledTransferQueries sellerScheduledTransfer;
        private readonly SellerService sellerService;
        private readonly SellerProfileValidationService sellerProfileValidation;
        private readonly IMessagesDispatcher messages;
        private readonly ApplicationSignInManager signInManager;
        private readonly ApplicationUserManager userManager;
        private readonly IAuthenticationManager authenticationManager;

        public SellersController(
            BatchImportAppService batchImportAppService, SellerService sellerService, SellerProfileValidationService sellerProfileValidation,
            ISellerRepository sellerRepository, ISellerScheduledTransferQueries sellerScheduledTransfer,
            IProductRepository productRepository, IUserRatingAppService ratingAppService,
            IRatingsQueries ratingsQueries, ICustomFieldAppService customFieldAppService,
            ILeadSalesAppService leadSalesAppService, SellerAppService sellerAppService, IMessagesDispatcher messages,
            ApplicationSignInManager signInManager, ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
        {
            this.batchImportAppService = batchImportAppService;
            this.sellerService = sellerService;
            this.sellerProfileValidation = sellerProfileValidation;
            this.sellerRepository = sellerRepository;
            this.sellerScheduledTransfer = sellerScheduledTransfer;
            this.productRepository = productRepository;
            this.ratingAppService = ratingAppService;
            this.ratingsQueries = ratingsQueries;
            this.customFieldAppService = customFieldAppService;
            this.leadSalesAppService = leadSalesAppService;
            this.sellerAppService = sellerAppService;
            this.messages = messages;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.authenticationManager = authenticationManager;
        }

        #region Import Sellers

        public ActionResult AddCredits(string sellerId, int amount, int? duration)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("amount", "Amount of credits must be more than zero");

            if (duration.HasValue && duration <= 0)
                throw new ArgumentOutOfRangeException("duration", "Validity must be at least one day");

            leadSalesAppService.AddCredits(sellerId, amount, duration.GetValueOrDefault(0));
            return RedirectToAction("Details", "Sellers", new { id = sellerId });
        }

        public ActionResult ApproveModeration(string sellerguid,bool isApproved)
        {
            var seller = sellerRepository.GetById(sellerguid);

            if (seller == null) return HttpNotFound();

            sellerService.SetEnabledStatus(seller.Id, isApproved);
            sellerService.ApproveModeration(seller);


            return RedirectToAction("Details", new { id = sellerguid });
        }

        public ActionResult ChangeEnabled(string id, bool enabled)
        {
            if (id == null) return HttpNotFound();

            sellerService.SetEnabledStatus(id, enabled);

            return RedirectToAction("Details", "Sellers", new { Id = id });
        }

        public ActionResult ChangeFees(string id, bool overrideGlobalPaymentFees, decimal? customPaymentFeePercent, decimal? customPaymentFeeAmount, bool customIsLiable)
        {
            if (id == null) return HttpNotFound();
            sellerService.SetCustomPaymentFees(id, overrideGlobalPaymentFees, customPaymentFeePercent, customPaymentFeeAmount, customIsLiable);
            return RedirectToAction("Details", "Sellers", new { Id = id });
        }

        public ActionResult ChangeProductStatus(string productId, string sellerId, bool disabled)
        {
            var product = productRepository.GetById(new Guid(productId));
            product.IsDisabled = disabled;
            productRepository.Update(product, true);
            return Details(sellerId);
        }

        public ActionResult Details(string id)
        {
            if (id == null) return HttpNotFound();

            var seller = sellerService.GetDetailsForAdmin(id);
            if (seller == null) return HttpNotFound();

            if (MaaSConfig.Current.Seller.UserRatings.Enabled)
            {
                ViewData.Add("AverageRating", ratingAppService.GetAverageRating(id, out int count));
                ViewData.Add("RatingsCount", count);

                var ratings = ratingsQueries.ListForAdmin(0, 100, out int num, id);
                ratings = ratings.Where(x => x.Rating.HasValue).ToList();
                ViewData.Add("RatingsList", ratings);
            }

            List<SellerScheduledTransferViewModel> sellerScheduledTransferView = new List<SellerScheduledTransferViewModel>();
            if (MaaSConfig.Current.Payment.GatewayIsPagarme() && MaaSConfig.Current.Payment.Common.EnableTransactionWarranty)
                sellerScheduledTransferView = sellerScheduledTransfer.GetScheduledTransferForAdmin(id);

            var model = new SellerModel { Seller = seller, SellerScheduledTransfer = sellerScheduledTransferView };

            if (MaaSConfig.Current.CustomFields.Seller.Enabled)
                customFieldAppService.ReadCustomFieldsToViewModel(model);

            if (MaaSConfig.Current.Quote.LeadSales.Enabled)
                model.Seller.LeadSalesProfile = leadSalesAppService.GetLeadSalesProfile(id);

            return View("Details", model);
        }

        public async Task<ActionResult> DownloadImportTemplate(string type)
        {
            var sourceType = GetSourceType(type);

            var stream = new MemoryStream();
            await batchImportAppService.GenerateSellerTemplateAsync(sourceType, stream);
            stream.Seek(0, SeekOrigin.Begin);

            switch (sourceType)
            {
                case BatchImportSourceType.Excel:
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "template.xlsx");

                case BatchImportSourceType.Csv:
                    return File(stream, "text/csv", "template.csv");

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ActionResult Import()
        {
            if (!MaaSConfig.Current.BatchImport.Sellers.Enabled)
                return new HttpNotFoundResult();

            return View(new ImportModel());
        }

        public ActionResult ImportResults()
        {
            if (!MaaSConfig.Current.BatchImport.Sellers.Enabled)
                return new HttpNotFoundResult();

            return RedirectToAction("Import");
        }

        [HttpPost]
        public async Task<ActionResult> ImportResults(HttpPostedFileBase file)
        {
            if (!MaaSConfig.Current.BatchImport.Sellers.Enabled)
                return new HttpNotFoundResult();

            var model = new ImportResultsModel
            {
                SourceType = Request.Form["SourceType"]
            };

            var parsedSourceType = GetSourceType(model.SourceType);

            try
            {
                if (file.ContentLength > 0)
                    model.Results = await batchImportAppService.ImportSellersAsync(
                        parsedSourceType, file.InputStream);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            model.HasModelErrors = !ModelState.IsValid;
            return View(model);
        }

        public async Task<ActionResult> Index(string mode = null)
        {
            IndexModel model;

            if ("moderation".Equals(mode, StringComparison.OrdinalIgnoreCase))
            {
                var sellers = await messages.QueryAsync(new ListSellersPendingModerationForAdmin());
                model = new IndexModel(sellers.Select(seller => SellerListItemModel.From(seller)));
            }
            else if ("inactive".Equals(mode, StringComparison.OrdinalIgnoreCase))
            {
                var sellers = sellerService.ListForAdmin().Where(seller => seller.Enabled == false);
                model = new IndexModel(sellers.Select(seller => SellerListItemModel.From(seller)));
            }
            else
            {
                var sellers = sellerService.ListForAdmin().Where(seller => seller.Enabled && (seller.RequireModeration == false || seller.RequireModeration == null));
                model = new IndexModel(sellers.Select(seller => SellerListItemModel.From(seller)));
            }

            return View(model);
        }

        public async Task<ActionResult> SignInAsSeller(string id)
        {
            var seller = sellerRepository.GetById(id);
            var user = await userManager.FindByIdAsync(seller.UserId);

            if (user != null)
            {
                authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                ControllerContext.HttpContext.Session.RemoveAll();
                await signInManager.SignInAsync(user, false, false);
                PreferredPersonIdHelper.Set(seller.Id);

                return RedirectToAction("Index", "Manage", new { area = "" });
            }

            this.SetErrorMessage(Request.GetLocalizedString("LOGIN_FAILURE_MESSAGE"));
            return RedirectToAction("Details", "Sellers", new { id });
        }

        private BatchImportSourceType GetSourceType(string type)
        {
            switch (type.ToLower())
            {
                case "excel":
                    return BatchImportSourceType.Excel;

                case "csv":
                    return BatchImportSourceType.Csv;

                default:
                    throw new ArgumentException($"Type '{type}' is not valid. There is not a matching BatchImportSourceType.");
            }
        }

        #endregion Import Sellers

        public ActionResult SellersValidation()
        {
            sellerProfileValidation.RunForAllSellers();
            return new HttpStatusCodeResult(200);
        }
    }
}