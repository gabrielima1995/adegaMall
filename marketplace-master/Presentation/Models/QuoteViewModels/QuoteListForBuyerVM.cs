using Application.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using V2.Application.Modules.QuoteRequests.Queries;

namespace Presentation.Models.QuoteViewModels
{
    public class QuoteListForBuyerVM
    {
        public List<QuoteListItemForBuyerVM> Items { get; set; }

        public int TotalQuotes => Items.Count();

        public QuoteListForBuyerVM() { }

        public QuoteListForBuyerVM(QuoteListForBuyer dto, bool isLoggedInAsSeller, bool isLoggedInAsBuyer)
        {
            Items = dto.Items.Select(item => new QuoteListItemForBuyerVM(item, isLoggedInAsSeller, isLoggedInAsBuyer)).ToList();
        }
    }

    public class QuoteListItemForBuyerVM
    {
        public QuoteVM Quote { get; set; }

        public bool IsGroup { get; set; }

        public bool AllClosed { get; set; }

        public int TotalSellersCount { get; set; }

        public int AdditionalSellersCount => TotalSellersCount - TopSellers.Count();

        public IEnumerable<QuoteListItemForBuyerTopSellerVM> TopSellers { get; set; }

        public QuoteListItemForBuyerInteractionVM LatestInteraction { get; set; }

        public QuoteListItemForBuyerVM() { }

        public QuoteListItemForBuyerVM(QuoteListItemForBuyer dto, bool isLoggedInAsSeller, bool isLoggedInAsBuyer)
        {
            Quote = new QuoteVM(dto.Quote, dto.Billing, isLoggedInAsSeller, isLoggedInAsBuyer);
            IsGroup = dto.IsGroup;
            TotalSellersCount = dto.TotalSellersCount;
            TopSellers = dto.TopSellers.Select(x => new QuoteListItemForBuyerTopSellerVM(x));
            LatestInteraction = dto.LatestInteraction == null ? null : new QuoteListItemForBuyerInteractionVM(dto.LatestInteraction);
            AllClosed = dto.AllClosed;
        }
    }

    public class QuoteListItemForBuyerInteractionVM
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public bool IsSeller { get; set; }

        public object SellerNickname { get; }

        [DataType(DataType.DateTime)]
        public DateTime OccurredAt { get; set; }

        public string Text { get; set; }

        public decimal? Price { get; set; }

        public DateTime? DeliveryDeadline { get; set; }

        public QuoteListItemForBuyerInteractionVM() { }

        public QuoteListItemForBuyerInteractionVM(QuoteListItemForBuyerInteraction dto)
        {
            UserId = dto.UserId;
            Name = dto.Name;
            AvatarUrl = ImgHelper.Avatar(dto.AvatarUrl, 100);
            IsSeller = dto.IsSeller;
            SellerNickname = dto.SellerNickname;
            OccurredAt = dto.OccurredAt;
            Text = dto.Text;
            Price = dto.Price;
            DeliveryDeadline = dto.DeliveryDeadline;
        }
    }

    public class QuoteListItemForBuyerTopSellerVM
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Nickname { get; private set; }

        public string AvatarUrl { get; set; }

        public QuoteListItemForBuyerTopSellerVM() { }

        public QuoteListItemForBuyerTopSellerVM(QuoteListItemForBuyerTopSeller dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Nickname = dto.Nickname;
            AvatarUrl = ImgHelper.Avatar(dto.AvatarUrl, 100);
        }
    }
}