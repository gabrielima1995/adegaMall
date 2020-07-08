using Application.Config;
using Domain.Entities.PortfolioModule;
using Domain.Entities.StoreModule;
using Domain.Entities.SubscriptionModule;
using Domain.Modules.Portfolio;
using Domain.Modules.StoreModule.CompleteProfile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Domain.Entities
{
    public class Seller : Person
    {
        private Seller()
        {
        }

        public Seller(string userId, string email, bool requireModeration) : base(userId, email, requireModeration)
        {
        }

        public string Nickname { get; set; }
        public string FacebookProfile { get; set; }
        public string LinkedInProfile { get; set; }
        public string InstagramProfile { get; set; }
        public string GooglePlusProfile { get; set; }
        public string WhatsAppNumber { get; set; }

        [Required]
        [StringLength(1000)]
        public string FriendlyUrl { get; set; }

        public Portfolio Portfolio { get; set; }
        public IList<Quote> Quotes { get; set; } = new List<Quote>();

        public Store Store { get; set; }
        public IList<Order> Orders { get; set; } = new List<Order>();

        public bool SubscriptionStarted { get; set; }
        public Subscription Subscription { get; set; }

        public string AccountableName { get; set; }
        public string AccountableDocumentNumber { get; set; }

        public override string AccountableDocument => AccountableDocumentNumber;

        public BankAccount BankAccount { get; set; }

        public bool HasSubscription => Subscription != null;
        public string PublicProfileName => AlternativeName ?? FullName;
        public bool IsValidSubscription => HasSubscription && Subscription.IsValid;

        public bool? GatewayTransferEnabled { get; set; }
        public bool? OverrideGlobalPaymentFees { get; set; }
        public bool? CustomIsLiable { get; set; }
        public decimal? CustomPaymentFeePercent { get; set; }
        public decimal? CustomPaymentFeeAmount { get; set; }

        public LeadSalesProfile LeadSalesProfile { get; set; }
        public string SellerCompleteProfile { get; private set; }
        public string SellerCompleteProfileVersion { get; set; }

        [NotMapped]
        public SellerCompleteProfile SellerCompleteProfileMapped
        {
            get => SellerCompleteProfile == null ? new SellerCompleteProfile() : JsonConvert.DeserializeObject<SellerCompleteProfile>(SellerCompleteProfile);
            set => SellerCompleteProfile = JsonConvert.SerializeObject(value);
        }

        public bool IsProfileComplete
        {
            get
            {
                bool _complete = true;

                if (MaaSConfig.Current.Payment.Enabled && string.IsNullOrWhiteSpace(SupplierGatewayId))
                    _complete = false;

                if (MaaSConfig.Current.Quote.Enabled && Portfolio?.SubCategories?.Count == 0)
                    _complete = false;

                return _complete;
            }
        }

        public void CalculateScore()
        {
            var parms = new Dictionary<string, bool>
            {
                {nameof(DateOfBirth), DateOfBirth != null},
                {nameof(Genre), Genre != null},
                {nameof(DocumentNumber), !string.IsNullOrEmpty(DocumentNumber)},
                {nameof(AdditionalInfo1), !string.IsNullOrEmpty(AdditionalInfo1)},
                {nameof(AdditionalInfo2), !string.IsNullOrEmpty(AdditionalInfo2)},
                {nameof(Avatar), HasAvatar},
                {nameof(Address), HasAddress},
                {nameof(SupplierGatewayId), IntegratorSupplierRegistered},
                {nameof(CustomerGatewayId), IntegratorCustomerRegistered},
                {nameof(AlternativeName), !string.IsNullOrEmpty(AlternativeName)},
                {nameof(FacebookProfile), !string.IsNullOrEmpty(FacebookProfile)},
                {nameof(LinkedInProfile), !string.IsNullOrEmpty(LinkedInProfile)},
                {nameof(InstagramProfile), !string.IsNullOrEmpty(InstagramProfile)},
                {nameof(GooglePlusProfile), !string.IsNullOrEmpty(GooglePlusProfile)},
                {nameof(AboutMe), !string.IsNullOrEmpty(AboutMe)}
            };

            Score = parms.Count(i => i.Value);
        }

        public decimal GetCurrentPaymentPercentFee()
        {
            var percent = MaaSConfig.Current.Payment.Common.GlobalStartupPercentageFee;

            if (OverrideGlobalPaymentFees.GetValueOrDefault())
            {
                if (CustomPaymentFeePercent.HasValue)
                    percent = CustomPaymentFeePercent.Value;
            }

            return percent;
        }

        public decimal GetCurrentPaymentAmountFee()
        {
            var amount = MaaSConfig.Current.Payment.Common.GlobalStartupAmountFee;

            if (OverrideGlobalPaymentFees.GetValueOrDefault())
            {
                if (CustomPaymentFeeAmount.HasValue)
                    amount = CustomPaymentFeeAmount.Value;
            }

            return amount;
        }

        public bool IsSellerLiable()
        {
            var isLiable = MaaSConfig.Current.Payment.Common.GlobalIsLiable;

            if (OverrideGlobalPaymentFees.GetValueOrDefault())
            {
                if (CustomIsLiable.HasValue)
                    isLiable = CustomIsLiable.Value;
            }

            return isLiable;
        }

        public void GenerateAlternativeName()
        {
            var config = MaaSConfig.Current.Seller.AlternativeName;

            if (!config.GenerateEncoded) return;

            var suffix = Guid.NewGuid().Substring(6);

            AlternativeName = string.Format(config.EncodedTemplate, suffix);
        }

        public void GenerateNickname()
        {
            var value = PublicProfileName
                .ReplaceDiacritics()
                .Hyphenize();

            if (value.Length > 22) value = value.Substring(0, 22);

            value += Guid.NewGuid().Substring();

            Nickname = value;
        }

        public void GenerateFriendlyUrl(string suffix = null)
        {
            var value = !string.IsNullOrEmpty(suffix)
                ? PublicProfileName + "-" + suffix
                : PublicProfileName;

            FriendlyUrl = value
                .ReplaceDiacritics()
                .Hyphenize();
        }

        public static Seller CreateAsDto() => new Seller();

        public override PersonType PersonType => PersonType.Seller;
    }
}