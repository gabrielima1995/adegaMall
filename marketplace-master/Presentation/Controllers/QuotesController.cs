using Application.Config;
using Application.Modules.Ratings;
using Domain.Categories;
using Domain.Core;
using Domain.DTO;
using Domain.Entities.PortfolioModule;
using Domain.Interfaces.Repositories;
using Domain.Modules.QuoteBillingModule.Entities;
using Domain.Modules.QuoteBillingModule.Interfaces;
using Domain.Modules.Quotes;
using Domain.Modules.Ratings;
using Domain.QuoteRequests;
using Domain.QuoteRequests.Commands;
using Framework;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Presentation.Models;
using Presentation.Models.QuoteViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.LeadSales;
using V2.Application.Modules.QuoteBillingModule;
using V2.Application.Modules.QuoteBillingModule.Queries;
using V2.Application.Modules.QuoteRequests;
using V2.Application.Modules.QuoteRequests.AppCommands;
using V2.Application.Modules.QuoteRequests.Queries;
using V2.Application.Modules.Search.Quotes;
using V2.Application.Modules.SellerAssistantModule;

namespace Presentation.Controllers
{
    [AuthorizeEnabled]
    public class QuotesController : BaseController
    {
        private static MaaSConfig Config => MaaSConfig.Current;
        private const string BuyerCookieName = "buyer_quote";

        private readonly IBuyerRepository _buyerRepository;
        private readonly ICustomFieldAppService _customFieldAppService;
        private readonly ILeadSalesAppService _leadSalesAppService;
        private readonly IQuoteAnswerRepository _quoteAnswerRepository;
        private readonly IQuoteBillingQueries _quoteBillingQueries;
        private readonly IQuoteBillingRepository _quoteBillingRepository;
        private readonly IQuoteQueries _quoteQueries;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IQuoteSearchService _quoteSearchService;
        private readonly ISellerRepository _sellerRepository;
        private readonly IUserRatingAppService _userRatingAppService;
        private readonly IUserRatingsRepository _userRatingsRepository;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly CategoryService _categoryService;
        private readonly QuoteAppService _quoteAppService;
        private readonly QuoteBillingAppService _quoteBillingAppService;
        private readonly QuoteService _quoteService;
        private readonly ClonePublicQuoteForSellerCommandHandler _clonePublicQuoteForSellerCommandHandler;

        public QuotesController(
            IBuyerRepository buyerRepository,
            ICustomFieldAppService customFieldAppService,
            ILeadSalesAppService leadSalesAppService,
            IQuoteAnswerRepository quoteAnswerRepository,
            IQuoteBillingQueries quoteBillingQueries,
            IQuoteBillingRepository quoteBillingRepository,
            IQuoteQueries quoteQueries,
            IQuoteRepository quoteRepository,
            IQuoteSearchService quoteSearchService,
            ISellerRepository sellerRepository,
            IUserRatingAppService userRatingAppService,
            IUserRatingsRepository userRatingsRepository,
            IDomainEventDispatcher domainEventDispatcher,
            CategoryService categoryService,
            QuoteAppService quoteAppService,
            QuoteBillingAppService quoteBillingAppService,
            QuoteService quoteService,
            ClonePublicQuoteForSellerCommandHandler clonePublicQuoteForSellerCommandHandler)
        {
            _buyerRepository = buyerRepository;
            _categoryService = categoryService;
            _customFieldAppService = customFieldAppService;
            _leadSalesAppService = leadSalesAppService;
            _quoteAnswerRepository = quoteAnswerRepository;
            _quoteAppService = quoteAppService;
            _quoteBillingAppService = quoteBillingAppService;
            _quoteBillingQueries = quoteBillingQueries;
            _quoteBillingRepository = quoteBillingRepository;
            _quoteQueries = quoteQueries;
            _quoteRepository = quoteRepository;
            _quoteSearchService = quoteSearchService;
            _quoteService = quoteService;
            _clonePublicQuoteForSellerCommandHandler = clonePublicQuoteForSellerCommandHandler;
            _sellerRepository = sellerRepository;
            _userRatingAppService = userRatingAppService;
            _userRatingsRepository = userRatingsRepository;
            _domainEventDispatcher = domainEventDispatcher;
        }

        [HttpPost]
        [AuthorizeBuyer]
        public ActionResult AcceptQuote(AcceptQuoteVM model)
        {
            if (!Config.Quote.BuyerDecision.Enabled)
                return HttpNotFound();

            var quote = _quoteService.GetById(model.QuoteId);
            var answer = _quoteAnswerRepository.GetById(model.QuoteAnswerId);

            if (answer.Quote_Id != quote.Id)
                return HttpNotFound();

            if (answer.Price == null || answer.DeliveryDeadline == null)
                throw new InvalidOperationException("Possible hacking attempt. Cannot accept quote from an answer without a proposal.");

            if (answer.DeliveryDeadline.Value <= DateTime.Today)
                return Content("CANNOT_ACCEPT_QUOTE_WITH_PAST_DEADLINE");

            var command = new AcceptQuoteCommand(
                model.QuoteId, answer.Price.Value, answer.DeliveryDeadline.Value);

            var result = _quoteService.AcceptQuoteAndCloseOthers(command);

            if (result.Errors.HasErrors)
                return Content(result.Errors.ToSimpleString());

            if (!MaaSConfig.Current.Quote.QuoteBilling.Enabled)
                CreateUserRatingsForQuote(model.QuoteId);

            return RedirectToAction("buyer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAssistantPermissions(SellerAssistantPermissions.QuotesEdit)]
        public ActionResult AddAnswer(QuoteAnswerVM answerViewModel)
        {
            if (!Config.Quote.Enabled) return HttpNotFound();

            var profile = GetLoginProfile();
            var authorId = GetPersonId();
            var command = new AddQuoteAnswerAppCommand
            {
                QuoteId = answerViewModel.Quote_Id,
                AnswerId = Guid.NewGuid(),
                BuyerId = answerViewModel.Buyer_Id,
                SellerId = answerViewModel.Seller_Id,
                AuthorId = authorId,
                Text = answerViewModel.Text,
                Price = answerViewModel.Price,
                Deadline = answerViewModel.DeliveryDeadline,
                Attachments = GetFileUploads(),
                IsModerator = profile.IsAdminOrGestao()
            };

            var result = _quoteAppService.AddAnswer(command);
            if (!result.OK) return Content(result.Errors.ToSimpleString());

            var quote = _quoteService.GetDetails(answerViewModel.Quote_Id);
            if (quote == null) return HttpNotFound();

            if (answerViewModel.CancelQuote)
                _quoteService.CloseQuote(new CloseQuoteCommand(answerViewModel.Quote_Id, false));

            var isSeller = profile.IsSellerOrSellerAssistant();
            var isDealWon = quote.IsClosed && quote.DealResult == QuoteDealResult.Won;

            var answersVM = quote.Answers.Select(answer => new QuoteAnswerVM(answer, isSeller, isDealWon)).ToList();
            var lastProposal = answersVM.LastOrDefault(answer => answer.IsProposal);
            if (lastProposal != null)
                lastProposal.IsLatestProposal = true;

            return PartialView("_DetailsAnswersList", answersVM);
        }

        [HttpGet]
        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.QuotesEdit)]
        public ActionResult AnswerPublicQuote(string groupId)
        {
            var parsedGroupId = new Guid(groupId);
            var sellerId = GetPersonId();

            if (!User.Identity.GetLoginProfile().SellerCompleteProfile.IsComplete)
                return RedirectToAction("Seller", "Manage");

            var quote = _quoteRepository.Get(x => x.GroupId == parsedGroupId && x.Seller_Id == sellerId);
            if (quote != null)
            {
                if (MaaSConfig.Current.Quote.LeadSales.Enabled && !_leadSalesAppService.IsQuoteUnlocked(sellerId, quote.Id.ToString()))
                    return RedirectToAction("ConfirmUnlock", new { quoteId = quote.Id });

                return RedirectToAction("Details", new { id = quote.Id });
            }

            var command = new ClonePublicQuoteForSellerCommand(parsedGroupId, Guid.NewGuid(), sellerId);
            _clonePublicQuoteForSellerCommandHandler.Handle(command);

            if (MaaSConfig.Current.Quote.LeadSales.Enabled)
                return RedirectToAction("ConfirmUnlock", new { quoteId = command.NewQuoteId });

            return RedirectToAction("Details", new { id = command.NewQuoteId });
        }

        [HttpGet]
        [AuthorizeBuyer]
        public ActionResult CancelGroup(Guid groupId)
        {
            var quoteIds = _quoteQueries.ListQuoteIdsInGroup(groupId);
            var commands = quoteIds.Select(id => new CloseQuoteCommand(id, dealWon: false));

            foreach (var command in commands)
                _quoteService.CloseQuote(command, isBuyer: true);

            return RedirectToAction("buyer");
        }

        [HttpGet]
        [AuthorizeBuyer]
        public ActionResult CancelQuote(Guid id)
        {
            if (!Config.Quote.BuyerDecision.Enabled)
                return HttpNotFound();

            var command = new CloseQuoteCommand(id, false);
            var result = _quoteService.CloseQuote(command, true);

            if (!result.OK)
                return Content(result.Errors.ToSimpleString());

            return RedirectToAction("buyer");
        }

        [HttpPost]
        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.QuotesEdit)]
        public ActionResult Close(CloseQuoteVM model)
        {
            if (!Config.Quote.Enabled)
                return HttpNotFound();

            var command = new CloseQuoteCommand(model.Id, model.DealWon);
            var result = _quoteService.CloseQuote(command);

            if (model.DealWon)
            {
                CreateUserRatingsForQuote(model.Id);
            }

            if (!result.OK)
                return Content(result.Errors.ToSimpleString());

            return new EmptyResult();
        }

        [HttpGet]
        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.QuotesEdit)]
        public ActionResult ConfirmUnlock(Guid quoteId)
        {
            var quote = _quoteRepository.GetById(quoteId);

            if (quote == null || !MaaSConfig.Current.Quote.LeadSales.Enabled)
                return HttpNotFound();

            var seller = _sellerRepository.GetWithLeadSalesProfile(GetPersonId());

            if (seller.LeadSalesProfile.IsQuoteUnlocked(quoteId))
                return RedirectToAction("Details", new { id = quoteId });

            var unlockVM = new UnlockQuoteVM()
            {
                CurrentCredits = seller.LeadSalesProfile.TotalAvailableCredits,
                QuoteId = quoteId,
                UnlockCost = 1,
            };

            return View("UnlockQuote", unlockVM);
        }

        [HttpPost]
        [AuthorizeSellerOrSellerAssistant]
        [RequireAssistantPermissions(SellerAssistantPermissions.QuotesEdit)]
        public ActionResult CreateBilling(Guid id)
        {
            _quoteBillingAppService.CreateBillingFromQuote(id);
            return RedirectToAction("Details", "Quotes", new { id });
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.QuotesView)]
        public ActionResult Details(Guid id, bool justUnlocked = false)
        {
            if (!Config.Quote.Enabled)
                return HttpNotFound();

            var profile = GetLoginProfile();

            if (profile.IsSellerOrSellerAssistant() && Config.Quote.LeadSales.Enabled && !_leadSalesAppService.IsQuoteUnlocked(GetPersonId(), id.ToString()))
                return HttpNotFound();

            var quote = _quoteService.GetDetails(id);

            if (quote == null)
                return HttpNotFound();

            QuoteBilling billing = null;

            if (quote.IsClosed && Config.Quote.QuoteBilling.Enabled)
                billing = _quoteBillingRepository.GetForQuote(id);

            var isSeller = profile.IsSellerOrSellerAssistant();
            var isBuyer = profile.IsBuyer();
            var isDealWon = quote.IsClosed && quote.DealResult == QuoteDealResult.Won;

            var quoteVM = new QuoteVM(quote, billing, isSeller, isBuyer);
            _customFieldAppService.ReadCustomFieldsToViewModel(quoteVM);

            var answersVM = quote.Answers.Select(answer => new QuoteAnswerVM(answer, isSeller, isDealWon)).ToList();

            if (profile.IsBuyer() && MaaSConfig.Current.Seller.UserRatings.Enabled)
            {
                quoteVM.UserRating = _userRatingsRepository.GetByQuoteId(quote.Id.ToString(), quote.Seller_Id);
                quoteVM.SellerAverageRating = _userRatingAppService.GetAverageRating(quoteVM.SellerId, out int count);
                quoteVM.SellerRatingCount = count;
            }

            if (profile.IsSellerOrSellerAssistant() && MaaSConfig.Current.Buyer.UserRatings.Enabled)
            {
                quoteVM.UserRating = _userRatingsRepository.GetByQuoteId(quote.Id.ToString(), quote.Buyer_Id);
                quoteVM.SellerAverageRating = _userRatingAppService.GetAverageRating(quoteVM.BuyerId, out int count);
                quoteVM.SellerRatingCount = count;
            }

            if (justUnlocked)
                ViewData.Add("unlockAlert", true);

            return View(new QuoteDetailsVM(quoteVM, answersVM));
        }

        [AuthorizeBuyer]
        public ActionResult GroupDetails(Guid id)
        {
            if (!Config.Quote.Enabled)
                return HttpNotFound();

            var profile = GetLoginProfile();

            var details = _quoteQueries.GetQuoteGroupDetailsForBuyer(id);
            var billing = _quoteBillingQueries.GetBillingByQuoteGroup(id);
            var isSeller = profile.IsSellerOrSellerAssistant();
            var isBuyer = profile.IsBuyer();

            return View(new QuoteGroupDetailsForBuyerVM(details, billing, isSeller, isBuyer));
        }

        [RequireAssistantPermissions(SellerAssistantPermissions.QuotesView)]
        public ActionResult Index(string view = "open" /*open|ongoing|closed*/, string filter = "")
        {
            if (!Config.Quote.Enabled)
                return HttpNotFound();

            if (GetLoginProfile().IsBuyer())
                return RedirectToAction("Buyer");

            var sellerId = GetPersonId();
            var buyerDecisionEnabled = MaaSConfig.Current.Quote.BuyerDecision.Enabled;
            var quoteBillingEnabled = MaaSConfig.Current.Quote.QuoteBilling.Enabled;

            var viewingOngoing = "ongoing".Equals(view, StringComparison.OrdinalIgnoreCase);
            var viewingClosed = "closed".Equals(view, StringComparison.OrdinalIgnoreCase);

            var allQuotes = _quoteService.List(null, sellerId, null);

            var openQuotes = allQuotes.Where(x => !x.IsClosed).ToList();

            var ongoingQuotes = allQuotes.Where(x => (x.IsClosed &&
                x.DealResult.Value == QuoteDealResult.Won &&
                x.Deadline.HasValue && x.Deadline.Value > DateTime.UtcNow)).ToList();

            var closedQuotes = allQuotes.Where(x => x.IsClosed &&
                (x.DealResult == QuoteDealResult.Lost ||
                !x.Deadline.HasValue || x.Deadline.Value < DateTime.UtcNow)).ToList();

            var filteredQuotes =
                viewingOngoing ? ongoingQuotes :
                viewingClosed ? closedQuotes : openQuotes;

            var quoteBillings = quoteBillingEnabled
                ? _quoteBillingQueries.ListBillingsForQuoteIds(filteredQuotes.Select(x => x.Id))
                : new List<QuoteBilling>();

            var unlockedQuotesList = _leadSalesAppService.ListUnlockedQuotes(sellerId);

            var profile = GetLoginProfile();
            var isSeller = profile.IsSellerOrSellerAssistant();
            var isBuyer = profile.IsBuyer();

            var listVM = new QuoteListVM(
                filteredQuotes, quoteBillings, openQuotes.Count,
                ongoingQuotes.Count, closedQuotes.Count, viewingOngoing, viewingClosed, unlockedQuotesList,
                isSeller, isBuyer);

            for (int i = listVM.Quotes.Count - 1; i >= 0; i--)
            {
                var quoteVM = listVM.Quotes[i];

                if (filter == "onlyUnlocked" && MaaSConfig.Current.Quote.LeadSales.Enabled)
                    if (!unlockedQuotesList.Contains(quoteVM.Id))
                        listVM.Quotes.RemoveAt(i);

                if (filter == "onlyLocked" && MaaSConfig.Current.Quote.LeadSales.Enabled)
                    if (unlockedQuotesList.Contains(quoteVM.Id))
                        listVM.Quotes.RemoveAt(i);

                _customFieldAppService.ReadCustomFieldsToViewModel(quoteVM);
                if (Config.Buyer.UserRatings.Enabled)
                {
                    quoteVM.SellerAverageRating = _userRatingAppService.GetAverageRating(quoteVM.BuyerId, out int count);
                    quoteVM.SellerRatingCount = count;
                }
            }

            return View(listVM);
        }

        [ActionName("Buyer")]
        public ActionResult IndexForBuyer()
        {
            if (!Config.Quote.Enabled)
                return HttpNotFound();

            var profile = GetLoginProfile();

            if (profile.IsSeller())
                return RedirectToAction("Index");

            var buyerId = GetPersonId();
            var quoteList = _quoteQueries.GetQuoteListForBuyer(buyerId);
            var isSeller = profile.IsSellerOrSellerAssistant();
            var isBuyer = profile.IsBuyer();

            var quoteListVM = new QuoteListForBuyerVM(quoteList, isSeller, isBuyer);

            if (MaaSConfig.Current.Seller.UserRatings.Enabled)
            {
                foreach (var item in quoteListVM.Items.Where(x => !x.IsGroup))
                {
                    var quote = item.Quote;
                    quote.SellerAverageRating = _userRatingAppService.GetAverageRating(quote.SellerId, out _);
                }
            }

            if (MaaSConfig.Current.CustomFields.Quotes.Enabled)
                foreach (var quoteVM in quoteListVM.Items)
                    _customFieldAppService.ReadCustomFieldsToViewModel(quoteVM.Quote);

            return View("IndexForBuyer", quoteListVM);
        }

        [AuthorizeBuyer]
        public ActionResult Pay(string billingId)
        {
            if (billingId == null)
                return HttpNotFound();

            if (!Guid.TryParse(billingId, out Guid parsedId))
                return HttpNotFound();

            _quoteBillingAppService.GetChekoutForBilling(parsedId);
            return RedirectToAction("Checkout", "Cart");
        }

        #region Search

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Search(string query, int? category, int? subCategory, string price, DateTime? minDate, QuoteSort ordering = QuoteSort.MostRecent, int page = 1)
        {
            if (!MaaSConfig.Current.Search.QuotesSearch.Enabled)
                return HttpNotFound();

            decimal? minPrice = null, maxPrice = null;
            if (price != null)
            {
                var prices = price.Split(',').Select(x => decimal.Parse(x)).ToList();
                minPrice = prices.Min();
                maxPrice = prices.Max();
            }

            var searchResult = _quoteSearchService.Search(new QuoteSearchVM()
            {
                Query = query,
                MinDate = minDate,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                OrderBy = ordering,
                Page = page - 1,
                Category = category,
                SubCategory = subCategory,
            }.ToSearchRequest());

            ViewBag.Categories = _categoryService.ListActiveCategories();
            ViewBag.SubCategories = new List<SubCategory>();
            if (category.HasValue)
            {
                ViewBag.SubCategories = _categoryService.ListActiveSubCategories(category);
            }

            ViewBag.CurrentPage = page;
            ViewBag.TotalCount = searchResult.TotalCount;
            if (MaaSConfig.Current.Quote.LeadSales.Enabled && GetLoginProfile().IsSellerOrSellerAssistant())
            {
                ViewBag.UnlockedGroups = _leadSalesAppService.ListUnlockedGroups(GetPersonId());
            }

            return View("Search/Index", searchResult.Items.ToList());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(QuoteSearchVM model, int page = 0)
        {
            if (!MaaSConfig.Current.Search.QuotesSearch.Enabled || model == null)
                return HttpNotFound();

            var searchResult = _quoteSearchService.Search(model.ToSearchRequest());
            ViewBag.CurrentPage = page;
            ViewBag.TotalCount = searchResult.TotalCount;

            return View("Search/Index", searchResult.Items.ToList());
        }

        [HttpGet]
        public ContentResult GetAddress()
        {
            var userId = User.Identity.GetLoginProfile();
            if (userId.PersonId == null)
                return null;

            var buyer = _buyerRepository.Get(x => x.Id == userId.PersonId, x => x.Address);
            if (buyer == null || buyer.Address == null)
                return null;

            var obj = new
            {
                buyer.Address.Street,
                buyer.Address.Number,
                buyer.Address.StreetNumber,
                buyer.Address.ZipCode,
                buyer.Address.City,
                buyer.Address.State,
                buyer.Address.CityAndState,
                buyer.Address.Neighborhood,
                buyer.Address.AdditionalInfo,
                buyer.Address.Complementary,
            };

            var address = JsonConvert.SerializeObject(obj);
            return Content(address, "application/json");
        }

        protected override IEnumerable<FileUpload> GetFileUploads()
        {
            var fileUploads = new List<FileUpload>();

            foreach (var fileName in Request.Files.AllKeys)
            {
                if (fileName != "quoteAttachment")
                    continue;

                var file = Request.Files[fileName];

                if (file.ContentLength == 0)
                    continue;

                fileUploads.Add(new FileUpload(file.InputStream, file.FileName, file.ContentType));
            }

            return fileUploads;
        }

        #endregion Search

        #region Helper Methods

        private OperationResult CreateUserRatingsForQuote(Guid quoteId)
        {
            var quote = _quoteRepository.GetById(quoteId);

            if (quote == null)
                return OperationResult.Fail("Invalid quote ID");

            bool hasFailed = false;

            Guid? rating1 = null, rating2 = null;

            //Buyer rates Seller
            if (MaaSConfig.Current.Seller.UserRatings.Enabled)
            {
                var result = _userRatingAppService.NewPendingRating(quote.Buyer_Id, quote.Seller_Id, "Seller", null, quote.Id.ToString());
                if (!result.OK)
                    hasFailed = true;

                else rating1 = result.Result;
            }

            //Seller rates Buyer
            if (MaaSConfig.Current.Buyer.UserRatings.Enabled)
            {
                var result = _userRatingAppService.NewPendingRating(quote.Seller_Id, quote.Buyer_Id, "Buyer", null, quote.Id.ToString());
                if (!result.OK)
                    hasFailed = true;

                else rating2 = result.Result;
            }

            if (hasFailed)
                return OperationResult.Fail("Error creating one or more Ratings");

            if (MaaSConfig.Current.Ratings.DoubleBlindRatings && rating1.HasValue && rating2.HasValue)
                _userRatingAppService.AssignRatingPairs(rating1.Value.ToString(), rating2.Value.ToString());

            return OperationResult.Success;
        }

        #endregion Helper Methods
    }
}