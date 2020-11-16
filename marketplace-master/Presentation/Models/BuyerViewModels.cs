using Application.Config;
using Application.Helpers;
using Domain.Entities;
using Domain.Utils;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Web;

namespace Presentation.Models
{
    public class BuyerProfileVM : IWithAddress, IWithCustomFields, IValidatableObject
    {
        private readonly MaaSConfig config = MaaSConfig.Current;
        private string _documentNumber;

        public string Id { get; set; }
        public string Lang { get; set; }
        public LoginMode LoginMode { get; set; }
        public DbGeography Location { get; set; }
        public HttpPostedFileBase NewPicture { get; set; }

        public bool BuyerMakesPayment => config.Payment.Enabled && (config.Product.Enabled || config.Quote.QuoteBilling.Enabled);
        public bool AddressEnabled => MaaSConfig.Current.Buyer.Address.Enabled || BuyerMakesPayment;
        public bool AddressRequired => AddressEnabled && (MaaSConfig.Current.Buyer.Address.IsRequired || (BuyerMakesPayment && MaaSConfig.Current.Payment.AntiFraud.Enabled));
        public bool CityAndStateRequired => MaaSConfig.Current.Buyer.CityAndState.IsRequired || AddressRequired;
        public bool DateOfBirthRequired => MaaSConfig.Current.Buyer.DateOfBirth.IsRequired;
        public bool DocumentNumberRequired => MaaSConfig.Current.Buyer.DocumentNumber.IsRequired || MaaSConfig.Current.Payment.Enabled;
        public bool GenreRequired => MaaSConfig.Current.Buyer.Genre.IsRequired;
        public bool PhoneNumberRequired => MaaSConfig.Current.Buyer.Phone.IsRequired;

        [DisplayNameLocalized("COMPLETE_NAME_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(1024)]
        public string Name { get; set; }

        [DisplayNameLocalized("EMAIL_LABEL")]
        [RequiredLocalized]
        [EmailAddressLocalized]
        [StringLengthLocalized(100)]
        public string Email { get; set; }

        [DisplayNameLocalized("PHONE_LABEL")]
        [StringLengthLocalized(15)]
        [RequiredIfLocalized("PhoneNumberRequired")]
        [PhoneNumberLocalized]
        public string PhoneNumber { get; set; }

        [DisplayNameLocalized("PICTURE_LABEL")]
        public string Picture { get; set; }

        [DisplayNameLocalized("DATE_OF_BIRTH_LABEL")]
        [RequiredIfLocalized("DateOfBirthRequired")]
        public DateTime? DateOfBirth { get; set; }

        [DisplayNameLocalized("GENRE_LABEL")]
        [RequiredIfLocalized("GenreRequired")]
        public Genre? Genre { get; set; }

        [DisplayNameLocalized("DOCUMENT_NUMBER_LABEL_BUYER")]
        [RequiredIfLocalized("DocumentNumberRequired")]
        public string DocumentNumber
        {
            get { return _documentNumber; }
            set { _documentNumber = MaaSConfig.Current.Buyer.DocumentNumber.AllowFreeText ? value : value?.ExtractDigits(); }
        }

        [DisplayNameLocalized("ADDRESS_LABEL")]
        [StringLengthLocalized(1024)]
        [RequiredIfLocalized("AddressRequired")]
        public string Address { get; set; }

        [DisplayNameLocalized("ADDRESS_NUMBER_LABEL")]
        [StringLengthLocalized(20)]
        [RequiredIfLocalized("AddressRequired")]
        public string AddressNumber { get; set; }

        [DisplayNameLocalized("NEIGHBORHOOD_LABEL")]
        [StringLengthLocalized(100)]
        [RequiredIfLocalized("AddressRequired")]
        public string AddressNeighborhood { get; set; }

        [DisplayNameLocalized("ZIPCODE_LABEL")]
        [StringLengthLocalized(15)]
        [RequiredIfLocalized("AddressRequired")]
        public string AddressZipCode { get; set; }

        [DisplayNameLocalized("ADDRESS_ADDITIONAL_INFO_LABEL")]
        [StringLengthLocalized(30)]
        public string AddressAdditionalInfo { get; set; }

        [DisplayNameLocalized("CITY_AND_STATE_LABEL")]
        [StringLengthLocalized(100)]
        [RequiredIfLocalized("CityAndStateRequired")]
        public string CityAndState { get; set; }

        public string AvatarUrl => ImgHelper.Avatar(Picture, 255);

        public bool RequireModeration { get; set; }

        [DisplayNameLocalized("TIME_ZONE_ID_LABEL")]
        public string PreferredTimeZoneId { get; set; } = TimeZones.Default.TimeZoneId;

        public CustomFieldEntityType EntityType => CustomFieldEntityType.Buyer;
        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }

        Guid IWithCustomFields.Id
        {
            get { return new Guid(Id); }
            set { Id = value.ToString(); }
        }

        public BuyerProfileVM()
        {
        }

        public BuyerProfileVM(Buyer buyer)
        {
            Name = buyer.FullName;
            Email = buyer.Email;
            PhoneNumber = buyer.PhoneNumber;
            Picture = buyer.Avatar;
            DateOfBirth = buyer.DateOfBirth;
            Genre = buyer.Genre;
            DocumentNumber = buyer.DocumentNumber;
            Address = buyer.Address?.Street;
            AddressNumber = buyer.Address?.Number;
            AddressNeighborhood = buyer.Address?.Neighborhood;
            AddressZipCode = buyer.Address?.ZipCode;
            AddressAdditionalInfo = buyer.Address?.AdditionalInfo;
            Location = buyer.Address?.Location;
            CityAndState = buyer.Address?.CityAndState;
            PreferredTimeZoneId = buyer.PreferredTimeZoneId;
            Id = buyer.Id;
            RequireModeration = buyer.RequireModeration ?? MaaSConfig.Current.Buyer.Moderation.Enabled;
        }

        public void CopyTo(Buyer buyer)
        {
            // Desativado em 28/04
            // Analisar para remover estas propriedades do Buyer
            // Agora, com login de comprador e vendedor unificados, isso deve ficar só em AspNetUsers
            //buyer.UserName = Email;
            //buyer.Email = Email;

            buyer.FullName = Name;
            buyer.PhoneNumber = PhoneNumber;

            buyer.DateOfBirth = DateOfBirth;
            buyer.Genre = Genre;

            if (string.IsNullOrEmpty(buyer.DocumentNumber) ||
                !DocumentNumberValidator.IsValidForBuyer(buyer.DocumentNumber))
                buyer.DocumentNumber = DocumentNumber;

            buyer.Address = buyer.Address ?? new Address();

            buyer.Address.Street = Address;
            buyer.Address.Number = AddressNumber;
            buyer.Address.Neighborhood = AddressNeighborhood;
            buyer.Address.ZipCode = AddressZipCode;
            buyer.Address.CityAndState = CityAndState;
            buyer.Address.AdditionalInfo = AddressAdditionalInfo;

            buyer.PreferredTimeZoneId = PreferredTimeZoneId;
            buyer.RequireModeration = MaaSConfig.Current.Buyer.Moderation.Enabled;
        }

        public BuyerUserAndProfile GetUserAndProfile()
        {
            var userAndProfile = BuyerUserAndProfile.Create(UserOrPersonId.Generate(), Email, Name,
                requireModeration: MaaSConfig.Current.Buyer.Moderation.Enabled);

            var buyer = userAndProfile.Profile;
            buyer.PhoneNumber = PhoneNumber;
            buyer.Avatar = Picture;
            buyer.DateOfBirth = DateOfBirth;
            buyer.Genre = Genre;
            buyer.DocumentNumber = DocumentNumber;

            buyer.Lang = Lang;
            buyer.PreferredTimeZoneId = PreferredTimeZoneId;
            buyer.ModifiedAt = DateTime.UtcNow;
            buyer.RequireModeration = RequireModeration;

            buyer.Address = new Address
            {
                Street = Address,
                Number = AddressNumber,
                Neighborhood = AddressNeighborhood,
                ZipCode = AddressZipCode,
                CityAndState = CityAndState,
                AdditionalInfo = AddressAdditionalInfo
            };

            return userAndProfile;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateOfBirth.HasValue)
            {
                int age = DateTime.Today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value > DateTime.Today.AddYears(-age)) age--;

                if (age < config.Buyer.DateOfBirth.MinAge)
                    yield return new ValidationResult(GlobalizationHelper.FormatString("BUYER_MIN_AGE_ERROR", config.Buyer.DateOfBirth.MinAge), new[] { nameof(DateOfBirth) });

                if (age > config.Buyer.DateOfBirth.MaxAge)
                    yield return new ValidationResult(GlobalizationHelper.FormatString("BUYER_MAX_AGE_ERROR", config.Buyer.DateOfBirth.MaxAge), new[] { nameof(DateOfBirth) });
            }
        }

        #region Obsolete

        [Obsolete("Use AvatarUrl instead")]
        public string GetPictureUrl()
        {
            return ImgHelper.Get(
                Picture ?? MaaSConfig.Current.Layout.Images.DefaultAvatar,
                width: 255, height: 222);
        }

        #endregion Obsolete
    }

    public class BuyerRegisterWithPasswordVM
    {
        public bool PhoneNumberRequired => MaaSConfig.Current.Buyer.Phone.IsRequired;

        [DisplayNameLocalized("COMPLETE_NAME_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(1024)]
        public string Name { get; set; }

        [DisplayNameLocalized("EMAIL_LABEL")]
        [RequiredLocalized]
        [EmailAddressLocalized]
        [StringLengthLocalized(100)]
        public string Email { get; set; }

        [DisplayNameLocalized("PHONE_LABEL")]
        [StringLengthLocalized(15)]
        [RequiredIfLocalized("PhoneNumberRequired")]
        [PhoneNumberLocalized]
        public string PhoneNumber { get; set; }

        [DisplayNameLocalized("PASSWORD_INPUT")]
        [RequiredLocalized]
        [DataType(DataType.Password)]
        [StringLengthLocalized(100, "PASSWORD_LENGTH_ERROR", MinimumLength = 6)]
        public string Password { get; set; }

        [DisplayNameLocalized("CONFIRM_PASSWORD_INPUT")]
        [DataType(DataType.Password)]
        [CompareLocalized("Password", "CONFIRM_PASSWORD_COMPARE_ERROR")]
        public string ConfirmPassword { get; set; }

        public string Lang { get; set; }

        [MustBeTrueLocalized("TERMS_NOT_AGREED_ERROR")]
        public bool Terms { get; set; }

        public BuyerUserAndProfile GetUserAndProfile()
        {
            var userAndProfile = BuyerUserAndProfile.Create(UserOrPersonId.Generate(), Email, Name,
                requireModeration: MaaSConfig.Current.Buyer.Moderation.Enabled);

            var buyer = userAndProfile.Profile;
            buyer.PhoneNumber = PhoneNumber;
            buyer.Lang = Lang;

            return userAndProfile;
        }
    }
}