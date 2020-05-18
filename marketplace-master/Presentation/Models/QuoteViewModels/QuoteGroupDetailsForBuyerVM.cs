using Application.Helpers;
using Domain.Modules.QuoteBillingModule.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using V2.Application.Modules.QuoteRequests.Queries;

namespace Presentation.Models.QuoteViewModels
{
    public class QuoteGroupDetailsForBuyerVM
    {
        public QuoteGroupDetailsForBuyerVM() { }
        public QuoteGroupDetailsForBuyerVM(QuoteGroupDetailsForBuyer details, QuoteBilling billing, bool isLoggedInAsSeller, bool isLoggedInAsBuyer)
        {
            Quote = new QuoteVM(details.Quote, billing, isLoggedInAsSeller, isLoggedInAsBuyer);
            SellerInteractions = details.SellerInteractions.Select(x => new QuoteGroupInteractionForBuyerVM(x));
            AllClosed = details.AllClosed;
            TotalSellersCount = details.TotalSellersCount;
        }

        public QuoteVM Quote { get; private set; }
        public IEnumerable<QuoteGroupInteractionForBuyerVM> SellerInteractions { get; private set; }
        public bool AllClosed { get; private set; }
        public int TotalSellersCount { get; private set; }
        public int AdditionalSellersCount => TotalSellersCount - SellerInteractions.Count();
        public bool HasAdditionalSellers => AdditionalSellersCount > 0;
    }

    public class QuoteGroupInteractionForBuyerVM
    {
        public QuoteGroupInteractionForBuyerVM() { }
        public QuoteGroupInteractionForBuyerVM(QuoteGroupInteractionForBuyer dto)
        {
            SellerId = dto.SellerId;
            QuoteId = dto.QuoteId;
            SellerName = dto.SellerName;
            SellerNickname = dto.SellerNickname;
            SellerAvatarUrl = ImgHelper.Avatar(dto.SellerAvatarUrl, 100);
            HasInteraction = dto.HasInteraction;
            InteractionText = dto.InteractionText;
            InteractionPrice = dto.InteractionPrice;
            InteractionDeadline = dto.InteractionDeadline;
            InteractionDate = dto.InteractionDate;
            AttachmentUrl = dto.AttachmentUrl;
            IsInteractionBySeller = dto.IsInteractionBySeller;
        }

        public string SellerId { get; set; }

        public Guid QuoteId { get; set; }

        public string SellerName { get; set; }

        public string SellerNickname { get; set; }

        public string SellerAvatarUrl { get; set; }

        public bool HasInteraction { get; set; }

        public string InteractionText { get; set; }

        public decimal? InteractionPrice { get; set; }

        [DataType(DataType.Date)]
        public DateTime? InteractionDeadline { get; set; }

        [DataType(DataType.Date)]
        public DateTime? InteractionDate { get; set; }

        public string AttachmentUrl { get; set; }

        public bool IsInteractionBySeller { get; set; }
    }
}