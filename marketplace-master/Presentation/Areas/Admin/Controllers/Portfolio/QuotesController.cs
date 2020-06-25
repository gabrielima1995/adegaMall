using Application.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.General.CustomFields;
using V2.Application.Modules.QuoteRequests.AppCommands;
using V2.Application.Modules.QuoteRequests.Model;
using V2.Application.Modules.QuoteRequests.Queries;

namespace Presentation.Areas.Admin.Controllers.Portfolio
{
    [AuthorizeAdmin]
    public class QuotesController : Controller
    {
        private const int pageSize = 100;

        private readonly IQuoteQueries quoteQueries;
        private readonly ICustomFieldAppService customFieldAppService;
        private readonly ClonePublicQuoteForSellerCommandHandler cloneQuoteCommandHandler;

        public QuotesController(
            IQuoteQueries quoteQueries,
            ICustomFieldAppService customFieldAppService,
            ClonePublicQuoteForSellerCommandHandler commandHandler)
        {
            this.quoteQueries = quoteQueries;
            this.customFieldAppService = customFieldAppService;
            this.cloneQuoteCommandHandler = commandHandler;
        }

        public ActionResult Index(int page = 0)
        {
            var quotes = quoteQueries.ListForAdmin(pageSize, page * pageSize, out int count);
            return View(new ResultList<QuoteListForAdminDto>(quotes, count, pageSize));
        }

        public ActionResult WithoutSeller(int page = 0)
        {
            var quotes = quoteQueries.ListWithoutSeller(pageSize, page * pageSize, out int count);
            return View(new ResultList<QuoteWithoutSellerListDto>(quotes, count, pageSize));
        }

        public ActionResult Details(Guid id)
        {
            var dto = quoteQueries.GetDetailsForAdmin(id);

            if (MaaSConfig.Current.CustomFields.Quotes.Enabled)
                customFieldAppService.ReadCustomFieldsToViewModel(dto);

            var sellersInQuoteGroup = ReadOnlyCollection.Empty<SellerGeneralDataDto>();
            var sellersToSendQuoteTo = ReadOnlyCollection.Empty<SellerGeneralDataDto>();

            if (dto.Quote.GroupId.HasValue &&
                MaaSConfig.Current.Quote.GeneralQuote.Enabled &&
                MaaSConfig.Current.Quote.GeneralQuote.RegisterQuoteEvenWhenNoSellers)
            {
                sellersInQuoteGroup = quoteQueries.ListSellersWhoReceivedQuoteByGroupId(dto.Quote.GroupId.Value);
                sellersToSendQuoteTo = quoteQueries.ListSellerToSendQuoteTo();
            }

            return View(new QuoteDetailsViewModel(dto, sellersInQuoteGroup, sellersToSendQuoteTo));
        }

        public ActionResult SendToSellers(Guid quoteId, Guid groupId, IEnumerable<string> sendToSellerIds)
        {
            if (sendToSellerIds != null)
            {
                var alreadyReceived = quoteQueries.ListSellersWhoReceivedQuoteByGroupId(groupId).Select(seller => seller.Id);
                sendToSellerIds = sendToSellerIds.Except(alreadyReceived);

                foreach (var sellerId in sendToSellerIds)
                {
                    var command = new ClonePublicQuoteForSellerCommand(groupId, Guid.NewGuid(), sellerId, treatAsNewQuote: true);
                    cloneQuoteCommandHandler.Handle(command);
                }
            }

            return RedirectToAction("Details", new { id = quoteId });
        }
    }

    public class ResultList<TItem>
    {
        public ResultList(IEnumerable<TItem> items, int totalCount, int pageSize)
        {
            Items = items.ToList().AsReadOnly();
            TotalCount = totalCount;
            PageSize = pageSize;
        }

        public ReadOnlyCollection<TItem> Items { get; }
        public int TotalCount { get; }
        public int PageSize { get; }
    }

    public class QuoteDetailsViewModel
    {
        public QuoteDetailsViewModel(
            QuoteDetailsForAdminDto dto,
            ReadOnlyCollection<SellerGeneralDataDto> sellersInQuoteGroup,
            ReadOnlyCollection<SellerGeneralDataDto> sellersToSendQuoteTo)
        {
            FixSellersAlternativeName(sellersInQuoteGroup);
            FixSellersAlternativeName(sellersToSendQuoteTo);

            Dto = dto;
            SellersInQuoteGroup = sellersInQuoteGroup;
            SellersToSendQuoteTo = new MultiSelectList(sellersToSendQuoteTo, nameof(SellerGeneralDataDto.Id), nameof(SellerGeneralDataDto.AlternativeName));
        }

        public QuoteDetailsForAdminDto Dto { get; }
        public ReadOnlyCollection<SellerGeneralDataDto> SellersInQuoteGroup { get; }
        public MultiSelectList SellersToSendQuoteTo { get; }

        public bool HasOriginalSeller => Dto.Seller != null;
        public bool HasSellersInQuoteGroup => SellersInQuoteGroup.Any();

        private void FixSellersAlternativeName(ReadOnlyCollection<SellerGeneralDataDto> sellers)
        {
            if (sellers == null)
                return;

            foreach (var seller in sellers)
                if (seller.AlternativeName == null)
                    seller.AlternativeName = seller.FullName;
        }
    }
}