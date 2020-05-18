using Application.Config;
using Application.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Modules.Portfolio;
using Domain.Modules.QuoteBillingModule.Entities;
using Domain.QuoteRequests;
using Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Presentation.Models
{
    public class QuoteVM : IWithCity, IWithCustomFields
    {
        private const int AvatarSize = 100;

        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? GroupId { get; set; }

        [DisplayNameLocalized("QUOTE_REGISTER_PASSWORD")]
        public string NewPassword { get; set; }

        [DisplayNameLocalized("QUOTE_REGISTER_PASSWORD_REPEAT")]
        [CompareLocalized("NewPassword", "CONFIRM_PASSWORD_COMPARE_ERROR")]
        public string ConfirmPassword { get; set; }

        public string SellerId { get; set; }

        public string SellerName { get; set; }

        public string SellerAvatar { get; set; }

        public string SellerUrl { get; set; }

        public double SellerAverageRating { get; set; }

        public int SellerRatingCount { get; set; }

        public string BuyerId { get; set; }

        [DisplayNameLocalized("NAME_LABEL")]
        [RequiredLocalized]
        public string BuyerName { get; set; }

        public string BuyerAvatar { get; set; }

        [DisplayNameLocalized("PHONE_LABEL")]
        [RequiredIfLocalized("PhoneForRegisterRequired")]
        public string BuyerPhone { get; set; }

        [DisplayNameLocalized("EMAIL_LABEL")]
        [RequiredLocalized]
        [EmailAddressLocalized]
        public string BuyerEmail { get; set; }

        [DisplayNameLocalized("QUOTE_TITLE_LABEL")]
        [RequiredLocalized]
        public string Title { get; set; }

        [DisplayNameLocalized("DESCRIPTION_LABEL")]
        [RequiredLocalized]
        public string Description { get; set; }

        [DisplayNameLocalized("CATEGORY_LABEL")]
        [RequiredIfLocalized("CategoriesRequired")]
        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        [DisplayNameLocalized("SUB_CATEGORIES_LABEL")]
        [RequiredIfLocalized("CategoriesRequired")]
        public int? SubCategoryId { get; set; }

        public string SubCategoryName { get; set; }

        public bool HasAddress => AddressStreet != null || CityAndState != null;

        [RequiredIfLocalized("AddressRequired")]
        [DisplayNameLocalized("ADDRESS_LABEL")]
        public string AddressStreet { get; set; }

        [RequiredIfLocalized("AddressRequired")]
        [DisplayNameLocalized("ADDRESS_NUMBER_LABEL")]
        public string AddressNumber { get; set; }

        [RequiredIfLocalized("AddressRequired")]
        [DisplayNameLocalized("NEIGHBORHOOD_LABEL")]
        public string AddressNeighborhood { get; set; }

        [RequiredIfLocalized("AddressRequired")]
        [DisplayNameLocalized("ZIPCODE_LABEL")]
        public string AddressZipCode { get; set; }

        [RequiredIfLocalized("CityAndStateRequired")]
        [DisplayNameLocalized("CITY_AND_STATE_LABEL")]
        public string CityAndState { get; set; }

        [RequiredIfLocalized("PhoneRequired")]
        [DisplayNameLocalized("PHONE_LABEL")]
        public string Phone { get; set; }

        [RequiredIfLocalized("DateRequired")]
        [DisplayNameLocalized("QUOTE_DATE_LABEL")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [DisplayNameLocalized("QUOTE_PREFERENCE_LABEL")]
        public QuotePreference Preference { get; set; }

        [RequiredIfLocalized("QuoteMaxRequired")]
        [DisplayNameLocalized("QUOTE_MAX_BUDGET_LABEL")]
        public decimal? Amount { get; set; }

        public Guid? PortfolioItemId { get; set; }

        [DisplayNameLocalized("QUOTE_PORTFOLIO_ITEM_LABEL")]
        public string PortfolioItemTitle { get; set; }

        [RequiredIfLocalized("QuantityRequired")]
        [RangeLocalized(1, int.MaxValue, "QUOTE_QUANTITY_ERROR")]
        [DisplayNameLocalized("QUOTE_QUANTITY_LABEL")]
        public int? Quantity { get; set; }

        public QuoteAnswerVM LatestAnswer { get; set; }

        public QuoteStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsClosed { get; set; }

        public DateTime? ClosedAt { get; set; }

        public bool DealWon { get; set; }

        public string AttachmentUrl { get; set; }

        public bool HasAnswerFromSeller { get; set; }

        public decimal? AcceptedValue { get; set; }

        public DateTime? Deadline { get; set; }

        public bool HasProposal => AcceptedValue != null && Deadline != null;

        public QuoteBilling Billing { get; set; }

        public bool IsLoggedInAsSeller { get; private set; }
        public bool IsLoggedInAsBuyer { get; private set; }

        public UserRating UserRating { get; set; }

        public string Location { get; set; }

        [RequiredIfLocalized(nameof(GeneralQuoteCityRequired))]
        public string Address { get; set; }

        public string Lat { get; set; }
        public string Lng { get; set; }
        public string BoundsAX { get; set; }
        public string BoundsAY { get; set; }
        public string BoundsBX { get; set; }
        public string BoundsBY { get; set; }

        public Dictionary<string, string> CustomFields { get; set; } = new Dictionary<string, string>();

        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; } = new Dictionary<Guid, CustomField>();

        public CustomFieldEntityType EntityType => CustomFieldEntityType.Quote;

        public bool QuoteDetailsHidden { get; set; }

        public bool HideBuyerDetailsUntilDealClosed { get; set; }

        public bool PhoneForRegisterRequired => MaaSConfig.Current.Quote.PhoneForRegister.IsRequired && !IsLoggedInAsBuyer;
        public bool PhoneRequired => (MaaSConfig.Current.Quote.Phone.IsRequired || MaaSConfig.Current.Seller.RequirePhoneForQuoteRegister.Enabled && !IsLoggedInAsBuyer) && !ShortQuoteEnabled;
        public bool DateRequired => MaaSConfig.Current.Quote.Date.IsRequired && !ShortQuoteEnabled;
        public bool AddressRequired => MaaSConfig.Current.Quote.Address.IsRequired && !ShortQuoteEnabled;
        public bool QuoteMaxRequired => MaaSConfig.Current.Quote.QuoteMax.IsRequired && !ShortQuoteEnabled;
        public bool CityAndStateRequired => (MaaSConfig.Current.Quote.Address.IsRequired || MaaSConfig.Current.Quote.CityAndState.IsRequired) && !ShortQuoteEnabled;
        public bool GeneralQuoteCityRequired => MaaSConfig.Current.Quote.GeneralQuoteCity.IsRequired && !IsSearchQuoteRequest && !IsSpecificQuoteRequest;
        public bool CategoriesRequired => !ShortQuoteEnabled && !PortfolioItemQuoteEnabled;
        public bool QuantityRequired => MaaSConfig.Current.Quote.PortfolioItemQuote.Quantity.IsRequired && PortfolioItemQuoteEnabled;

        public bool IsSearchQuoteRequest => !string.IsNullOrWhiteSpace(SearchQueryString);
        public bool IsGeneralQuoteRequest => string.IsNullOrEmpty(SellerId);
        public bool IsSpecificQuoteRequest => !string.IsNullOrEmpty(SellerId);
        public bool IsPortfolioItemQuoteRequest => PortfolioItemId.HasValue;

        public bool ShortQuoteEnabled =>
            IsSpecificQuoteRequest && !IsPortfolioItemQuoteRequest &&
            MaaSConfig.Current.Quote.SpecificQuote.Enabled &&
            MaaSConfig.Current.Quote.SpecificQuote.ShortQuote.Enabled;

        public bool PortfolioItemQuoteEnabled =>
            IsPortfolioItemQuoteRequest &&
            MaaSConfig.Current.Quote.PortfolioItemQuote.Enabled;

        public bool ShowPhoneForRegister => MaaSConfig.Current.Quote.PhoneForRegister.Enabled;
        public bool ShowPhone => (MaaSConfig.Current.Quote.Phone.Enabled || MaaSConfig.Current.Seller.RequirePhoneForQuoteRegister.Enabled && !IsLoggedInAsBuyer) && !ShortQuoteEnabled;
        public bool ShowDate => MaaSConfig.Current.Quote.Date.Enabled && !ShortQuoteEnabled;
        public bool ShowAddress => MaaSConfig.Current.Quote.Address.Enabled && !ShortQuoteEnabled;
        public bool ShowQuoteMax => MaaSConfig.Current.Quote.QuoteMax.Enabled && !ShortQuoteEnabled;
        public bool ShowCityAndState => (MaaSConfig.Current.Quote.Address.Enabled || MaaSConfig.Current.Quote.CityAndState.Enabled) && !ShortQuoteEnabled;
        public bool ShowGeneralQuoteCity => MaaSConfig.Current.Quote.GeneralQuoteCity.Enabled && !IsSearchQuoteRequest && !IsSpecificQuoteRequest;
        public bool ShowCategories => !ShortQuoteEnabled && !PortfolioItemQuoteEnabled;
        public bool ShowQuantity => MaaSConfig.Current.Quote.PortfolioItemQuote.Quantity.Enabled && PortfolioItemQuoteEnabled;
        public bool ShowPreference => MaaSConfig.Current.Quote.Preference.Enabled && !ShortQuoteEnabled;
        public bool ShowCustomFields => MaaSConfig.Current.CustomFields.Quotes.Enabled && CustomFieldDefinitions != null && !ShortQuoteEnabled;
        public bool ShowAttachment => MaaSConfig.Current.Quote.Attachment.Enabled && !ShortQuoteEnabled;

        public string SearchQueryString { get; set; }
        public int SearchSellerCount { get; set; }
        public List<string> SearchSellerAvatars { get; set; }

        public bool IsShortQuote { get; set; }

        public QuoteVM()
        {
        }

        public QuoteVM(Quote quote, QuoteBilling billing, bool isLoggedInAsSeller, bool isLoggedInAsBuyer)
        {
            if (quote == null)
                return;

            Billing = billing;
            Id = quote.Id;
            GroupId = quote.GroupId;
            Status = quote.Status;
            CreatedAt = quote.CreatedAt;

            WithSeller(quote.Seller);
            WithBuyer(quote.Buyer);

            Title = quote.Title;
            Description = quote.Description;
            CategoryId = quote.Category?.Id;
            CategoryName = quote.Category?.Name;
            SubCategoryId = quote.SubCategory?.Id;
            SubCategoryName = quote.SubCategory?.Name;
            AddressStreet = quote.Address?.Street;
            AddressNumber = quote.Address?.Number;
            AddressNeighborhood = quote.Address?.Neighborhood;
            AddressZipCode = quote.Address?.ZipCode;
            CityAndState = quote.Address?.CityAndState;
            Phone = quote.Phone;
            Date = quote.Date;
            Preference = quote.Preference;
            Amount = quote.Amount;
            Quantity = quote.Quantity;

            WithPortfolioItem(quote.PortfolioItem);

            IsClosed = quote.IsClosed;
            ClosedAt = quote.ClosedAt;
            DealWon = quote.DealResult == QuoteDealResult.Won;

            IsLoggedInAs(isLoggedInAsSeller, isLoggedInAsBuyer);

            AttachmentUrl = quote.Attachments?.FirstOrDefault()?.AttachmentUrl;
            LatestAnswer = quote.LatestAnswer == null ? null : new QuoteAnswerVM(quote.LatestAnswer, IsLoggedInAsSeller, DealWon);
            if (LatestAnswer != null)
            {
                LatestAnswer.SellerAvatar = SellerAvatar;
                LatestAnswer.BuyerAvatar = BuyerAvatar;
            }
            HasAnswerFromSeller = quote.HasAnswerFromSeller || (quote.Answers.Any(a => a.AuthorId == a.Seller_Id));

            Deadline = quote.Deadline;
            AcceptedValue = quote.AcceptedPrice;

            if (MaaSConfig.Current.Quote.HideBuyerDetailsUntilDealClosed)
            {
                if (!quote.IsClosed || quote.DealResult != QuoteDealResult.Won)
                    HideBuyerDetailsUntilDealClosed = true;
            }

            IsShortQuote = quote.IsShortQuote;
        }

        public QuoteVM WithSeller(Seller seller)
        {
            if (seller != null)
            {
                SellerId = seller.Id;
                SellerName = seller.PublicProfileName;
                SellerUrl = seller.FriendlyUrl;
                SellerAvatar = ImgHelper.Avatar(seller.Avatar, AvatarSize);
            }

            return this;
        }

        public QuoteVM WithBuyer(LoginProfile login)
        {
            if (login != null)
            {
                BuyerId = login.PersonId;
                BuyerName = login.Name;
                BuyerEmail = login.Email;
                BuyerPhone = login.PhoneNumber;
                IsLoggedInAsBuyer = true;
            }
            else
            {
                IsLoggedInAsBuyer = false;
            }

            return this;
        }

        public QuoteVM WithBuyer(Buyer buyer)
        {
            if (buyer != null)
            {
                BuyerId = buyer.Id;
                BuyerName = buyer.Name;
                BuyerEmail = buyer.Email;
                BuyerAvatar = ImgHelper.Avatar(buyer.Avatar, AvatarSize);
                BuyerPhone = buyer.PhoneNumber;
            }

            return this;
        }

        public QuoteVM IsLoggedInAs(bool isSeller, bool isBuyer)
        {
            IsLoggedInAsSeller = isSeller;
            IsLoggedInAsBuyer = isBuyer;

            return this;
        }

        public QuoteVM WithPortfolioItem(PortfolioItem item)
        {
            PortfolioItemId = item?.Id;
            PortfolioItemTitle = item?.Title;

            return this;
        }
    }

    public class QuoteDetailsVM : QuoteVM
    {
        public List<QuoteAnswerVM> Answers { get; set; } = new List<QuoteAnswerVM>();

        public QuoteDetailsVM(QuoteVM quote, List<QuoteAnswerVM> answers)
        {
            SimpleMapper.Map(quote, this);
            Answers = answers;

            var latestProposalAnswer = answers.FindLast(i => i.IsProposal);
            if (latestProposalAnswer != null)
                latestProposalAnswer.IsLatestProposal = true;
        }
    }
}