using Application.Config;
using Application.Helpers;
using Domain.Entities.PortfolioModule;
using Infra.Crosscutting.Globalization;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using V2.Application.Modules.QuoteRequests.Model;

namespace Presentation.Models
{
    public class QuoteAnswerVM
    {
        private readonly bool isDealWon;

        private bool HideAuthorDetails => MaaSConfig.Current.Quote.HideBuyerDetailsUntilDealClosed && IsLoggedInAsSeller && AuthorId == Buyer_Id && !isDealWon;

        public Guid Id { get; set; }

        [RequiredLocalized]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        public bool PriceRequired => MaaSConfig.Current.Quote.PriceInAnswerRequired;

        [DataType(DataType.Currency)]
        [DisplayNameLocalized("QUOTE_ANSWER_PRICE_INPUT_V2")]
        [RequiredIfLocalized("PriceRequired")]
        public decimal? Price { get; set; }

        public string AuthorId { get; set; }

        public string AuthorName =>
            IsModerator
                ? GlobalizationHelper.GetString("QUOTE_ANSWER_MODERATOR_LABEL")
                : HideAuthorDetails
                    ? GlobalizationHelper.GetString("BUYER_DETAILS_HIDDEN_UNTIL_DEAL_CLOSED")
                    : AuthorId == Buyer_Id
                        ? BuyerName
                        : SellerName;

        public string AuthorAvatar =>
            IsModerator
                ? MaaSConfig.Current.Layout.Images.ModeratorAvatar
                : HideAuthorDetails
                    ? MaaSConfig.Current.Layout.Images.DefaultAvatar
                    : AuthorId == Buyer_Id
                        ? BuyerAvatar
                        : SellerAvatar;

        public Guid Quote_Id { get; set; }

        public Guid? QuoteGroupId { get; set; }

        public string Buyer_Id { get; set; }

        public string Seller_Id { get; set; }

        public string BuyerName { get; set; }

        public string BuyerAvatar { get; set; }

        public string SellerName { get; set; }

        public string SellerAvatar { get; set; }

        public string AttachmentUrl { get; set; }

        public bool CancelQuote { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime AnswerDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DeliveryDeadline { get; set; }

        public bool IsProposal { get { return DeliveryDeadline.HasValue && Price.HasValue; } }

        public bool IsLatestProposal { get; set; }
        public bool IsLoggedInAsSeller { get; }

        public bool IsModerator => AuthorId != Buyer_Id && AuthorId != Seller_Id;

        public QuoteAnswerVM()
        {
        }

        public QuoteAnswerVM(QuoteDetailsVM quote, bool isLoggedInAsSeller, bool isDealWon)
        {
            if (quote == null) return;

            Quote_Id = quote.Id;
            Buyer_Id = quote.BuyerId;
            Seller_Id = quote.SellerId;
            QuoteGroupId = quote.GroupId;
            IsLoggedInAsSeller = isLoggedInAsSeller;
            this.isDealWon = isDealWon;
        }

        public QuoteAnswerVM(QuoteDetailsForAdminDto quote, bool isLoggedInAsSeller, bool isDealWon)
        {
            if (quote == null) return;

            Quote_Id = quote.Id;
            Buyer_Id = quote.Buyer.Id;
            if (quote.Seller != null)
                Seller_Id = quote.Seller.Id;
            QuoteGroupId = quote.Quote.GroupId;
            IsLoggedInAsSeller = isLoggedInAsSeller;
            this.isDealWon = isDealWon;
        }

        public QuoteAnswerVM(QuoteAnswer answer, bool isLoggedInAsSeller, bool isDealWon)
        {
            if (answer == null) return;

            var defaultAvatar = MaaSConfig.Current.Layout.Images.DefaultAvatar;

            Id = answer.Id;
            Text = answer.Text;
            Price = answer.Price;
            AuthorId = answer.AuthorId;
            Quote_Id = answer.Quote_Id;
            Buyer_Id = answer.Buyer_Id;
            BuyerName = answer.Buyer?.AlternativeName ?? answer.Buyer?.FullName;
            BuyerAvatar = ImgHelper.Get(answer.Buyer?.Avatar, 100, 100, defaultAvatar);
            Seller_Id = answer.Seller_Id;
            SellerName = answer.Seller?.AlternativeName ?? answer.Seller?.FullName;
            SellerAvatar = ImgHelper.Get(answer.Seller?.Avatar, 100, 100, defaultAvatar);
            AnswerDate = answer.CreatedAt;
            AttachmentUrl = answer.Attachments?.FirstOrDefault()?.AttachmentUrl;
            DeliveryDeadline = answer.DeliveryDeadline;
            IsLoggedInAsSeller = isLoggedInAsSeller;
            this.isDealWon = isDealWon;
        }
    }
}