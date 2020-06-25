using Application.Addresses;
using Application.Config;
using Application.Helpers;
using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Services;
using Framework;
using Geocoding;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Web;
using Address = Domain.Entities.Address;

namespace Application.Sellers
{
    public class SellerProfileViewModel : IWithAddress, IWithCustomFields, IValidatableObject
    {
        private string _documentNumber;

        public string Id { get; set; }

        [DisplayNameLocalized("COMPLETE_NAME_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(1024)]
        public string Name { get; set; }

        [DisplayNameLocalized("NICKNAME_LABEL")]
        [StringLengthLocalized(50)]
        public string Nickname { get; set; }

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

        [DisplayNameLocalized("DOCUMENT_NUMBER_LABEL")]
        [RequiredIfLocalized("DocumentNumberRequired")]
        public string DocumentNumber
        {
            get => _documentNumber;
            set => _documentNumber = MaaSConfig.Current.Seller.DocumentNumber.AllowFreeText ? value : value?.ExtractDigits();
        }

        [DisplayNameLocalized("ADDITIONAL_INFO_1_LABEL")]
        [StringLengthLocalized(1024)]
        [RequiredIfLocalized("AdditionalInfo1Required")]
        public string AdditionalInfo1 { get; set; }

        [DisplayNameLocalized("ADDITIONAL_INFO_2_LABEL")]
        [StringLengthLocalized(1024)]
        [RequiredIfLocalized("AdditionalInfo2Required")]
        public string AdditionalInfo2 { get; set; }

        public string AddressString { get; set; }

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

        public string AddressLat { get; set; }
        public string AddressLng { get; set; }

        [DisplayNameLocalized("ALTERNATIVE_LABEL")]
        [StringLengthLocalized(1024)]
        [RequiredIfLocalized("AlternativeNameRequired")]
        public string AlternativeName { get; set; }

        [DisplayNameLocalized("ABOUT_ME_LABEL")]
        [StringLengthLocalized(4000)]
        [RequiredIfLocalized("AboutMeRequired")]
        public string AboutMe { get; set; }

        [DisplayNameLocalized("FACEBOOK_LABEL")]
        [RequiredIfLocalized("FacebookProfileRequired")]
        public string FacebookProfile { get; set; }

        [DisplayNameLocalized("INSTAGRAM_LABEL")]
        [RequiredIfLocalized("InstagramProfileRequired")]
        [InstagramLocalized]
        public string InstagramProfile { get; set; }

        [DisplayNameLocalized("LINKEDIN_LABEL")]
        [RequiredIfLocalized("LinkedInProfileRequired")]
        [LinkedInLocalized]
        public string LinkedInProfile { get; set; }

        [DisplayNameLocalized("GOOGLEPLUS_LABEL")]
        [RequiredIfLocalized("GooglePlusProfileRequired")]
        [GooglePlusLocalized]
        public string GooglePlusProfile { get; set; }

        [DisplayNameLocalized("WHATSAPP_LABEL")]
        [RequiredIfLocalized("WhatsAppNumberRequired")]
        [StringLengthLocalized(21)]
        public string WhatsAppNumber { get; set; }

        [DisplayNameLocalized("PROFILE_URL_LABEL")]
        public string ProfileUrl { get; set; }

        public string FriendlyUrl { get; set; }

        public bool RequireModeration { get; set; }

        public LoginMode LoginMode { get; set; }
        public string Lang { get; set; }
        public HttpPostedFileBase NewPicture { get; set; }
        public DbGeography Location { get; set; }

        [DisplayNameLocalized("TIME_ZONE_ID_LABEL")]
        public string PreferredTimeZoneId { get; set; } = TimeZones.Default.TimeZoneId;

        public bool GatewayRegistered { get; set; }

        public bool PhoneNumberRequired => MaaSConfig.Current.Seller.Phone.IsRequired;
        public bool DateOfBirthRequired => MaaSConfig.Current.Seller.DateOfBirth.IsRequired;
        public bool GenreRequired => MaaSConfig.Current.Seller.Genre.IsRequired;
        public bool DocumentNumberRequired => MaaSConfig.Current.Seller.DocumentNumber.IsRequired || SellerReceivesPayment;
        public bool AdditionalInfo1Required => MaaSConfig.Current.Seller.AdditionalInfo1.IsRequired;
        public bool AdditionalInfo2Required => MaaSConfig.Current.Seller.AdditionalInfo2.IsRequired;
        public bool AddressRequired => (MaaSConfig.Current.Seller.Address.IsRequired || SellerReceivesPayment) && !MaaSConfig.Current.Seller.SavePartialProfiles.Enabled;
        public bool CityAndStateRequired => (MaaSConfig.Current.Seller.CityAndState.IsRequired || MaaSConfig.Current.Seller.Address.IsRequired) && !MaaSConfig.Current.Seller.SavePartialProfiles.Enabled;
        public bool AlternativeNameRequired => MaaSConfig.Current.Seller.AlternativeName.IsRequired && !MaaSConfig.Current.Seller.AlternativeName.GenerateEncoded;

        public bool AboutMeRequired => MaaSConfig.Current.Seller.AboutMe.IsRequired;
        public bool FacebookProfileRequired => MaaSConfig.Current.Seller.FacebookProfile.IsRequired;
        public bool InstagramProfileRequired => MaaSConfig.Current.Seller.InstagramProfile.IsRequired;
        public bool LinkedInProfileRequired => MaaSConfig.Current.Seller.LinkedInProfile.IsRequired;
        public bool GooglePlusProfileRequired => MaaSConfig.Current.Seller.GooglePlusProfile.IsRequired;
        public bool WhatsAppNumberRequired => MaaSConfig.Current.Seller.WhatsAppNumber.IsRequired;

        public bool SellerReceivesPayment => SellerReceivesMoipPayment || SellerReceivesPagarmePayment;
        public bool SellerReceivesMoipPayment => MaaSConfig.Current.Payment.Moip.SellerReceivesMoipPayment;
        public bool SellerReceivesPagarmePayment => MaaSConfig.Current.Payment.PagarMe.SellerReceivesPagarmePayment;

        public string AvatarUrl => ImgHelper.Avatar(Picture, 255);

        public CustomFieldEntityType EntityType => CustomFieldEntityType.Seller;
        public Dictionary<Guid, CustomField> CustomFieldDefinitions { get; set; }
        public Dictionary<string, string> CustomFields { get; set; }

        Guid IWithCustomFields.Id
        {
            get { return new Guid(Id); }
            set { Id = value.ToString(); }
        }

        public SellerProfileViewModel()
        {
        }

        public SellerProfileViewModel(Seller seller)
        {
            Id = seller.Id;

            Name = seller.FullName;
            Nickname = seller.Nickname;
            Email = seller.Email;
            PhoneNumber = seller.PhoneNumber;
            Picture = seller.Avatar;
            DateOfBirth = seller.DateOfBirth;
            Genre = seller.Genre;
            DocumentNumber = seller.DocumentNumber;
            AdditionalInfo1 = seller.AdditionalInfo1;
            AdditionalInfo2 = seller.AdditionalInfo2;

            AddressString = seller.Address?.ToString();
            Address = seller.Address?.Street;
            AddressNumber = seller.Address?.Number;
            AddressNeighborhood = seller.Address?.Neighborhood;
            AddressZipCode = seller.Address?.ZipCode;
            AddressAdditionalInfo = seller.Address?.AdditionalInfo;
            CityAndState = seller.Address?.CityAndState;

            Location = seller.Address?.Location;
            AddressLat = Location?.Latitude?.ToString(CultureInfo.InvariantCulture);
            AddressLng = Location?.Longitude?.ToString(CultureInfo.InvariantCulture);

            AlternativeName = seller.AlternativeName ?? seller.FullName;
            FacebookProfile = seller.FacebookProfile;
            LinkedInProfile = seller.LinkedInProfile;
            InstagramProfile = seller.InstagramProfile;
            GooglePlusProfile = seller.GooglePlusProfile;
            WhatsAppNumber = seller.WhatsAppNumber;
            AboutMe = seller.AboutMe;

            FriendlyUrl = seller.FriendlyUrl;
            ProfileUrl = seller.GetProfileUrl();
            PreferredTimeZoneId = seller.PreferredTimeZoneId;

            RequireModeration = seller.RequireModeration ?? MaaSConfig.Current.Seller.Moderation.Enabled;
            GatewayRegistered = !string.IsNullOrEmpty(seller.SupplierGatewayId);
        }

        public SellerUserAndProfile GetUserAndProfile()
        {
            var userAndProfile = SellerUserAndProfile.Create(UserOrPersonId.Generate(), Email, requireModeration: MaaSConfig.Current.Seller.Moderation.Enabled);
            var seller = userAndProfile.Profile;

            seller.FullName = Name;
            seller.PhoneNumber = PhoneNumber;
            seller.Avatar = Picture;
            seller.DateOfBirth = DateOfBirth;
            seller.Genre = Genre;
            seller.DocumentNumber = DocumentNumber;
            seller.AdditionalInfo1 = AdditionalInfo1;
            seller.AdditionalInfo2 = AdditionalInfo2;

            seller.AlternativeName = AlternativeName;
            seller.FacebookProfile = FacebookProfile;
            seller.LinkedInProfile = LinkedInProfile;
            seller.InstagramProfile = InstagramProfile;
            seller.GooglePlusProfile = GooglePlusProfile;
            seller.WhatsAppNumber = WhatsAppNumber;
            seller.AboutMe = AboutMe;

            seller.Lang = Lang;
            seller.PreferredTimeZoneId = PreferredTimeZoneId;
            seller.RequireModeration = MaaSConfig.Current.Seller.Moderation.Enabled;
            seller.ModifiedAt = DateTime.UtcNow;

            seller.Address = new Address
            {
                Street = Address,
                Number = AddressNumber,
                Neighborhood = AddressNeighborhood,
                ZipCode = AddressZipCode,
                CityAndState = CityAndState,
                AdditionalInfo = AddressAdditionalInfo
            };

            seller.CalculateScore();
            return userAndProfile;
        }

        public void Update(Seller seller,
            ICloudStorage cloudStorage, IGeocoder geocoder,
            FriendlyUrlGenerator friendlyUrlGenerator)
        {
            if (NewPicture != null && NewPicture.ContentLength > 0 && MaaSConfig.Current.Seller.ChangeAvatar)
            {
                try
                {
                    var uri = cloudStorage.UploadFile(CloudFileType.UserImage.ToString(), NewPicture.FileName, NewPicture.ContentType, NewPicture.InputStream);
                    seller.Avatar = uri.ToString();
                }
                catch { }
            }

            // Atualização de email e username desabilitados em 28/04
            // Username e email devem ser os mesmos do usuário (login)
            // Na verdade, Person nem deveria ter Email e Username
            //seller.UserName = Email;
            //seller.Email = Email;

            seller.FullName = Name;

            if (!MaaSConfig.Current.Seller.AlternativeName.GenerateEncoded)
                seller.AlternativeName = AlternativeName;
            
            seller.PhoneNumber = PhoneNumber;
            seller.AboutMe = AboutMe;

            seller.DateOfBirth = DateOfBirth;
            seller.Genre = Genre;
            seller.DocumentNumber = DocumentNumber;
            seller.AdditionalInfo1 = AdditionalInfo1;
            seller.AdditionalInfo2 = AdditionalInfo2;

            seller.Address = seller.Address ?? new Address();
            seller.Address.Street = Address;
            seller.Address.Number = AddressNumber;
            seller.Address.Neighborhood = AddressNeighborhood;
            seller.Address.ZipCode = AddressZipCode;
            seller.Address.CityAndState = CityAndState;
            seller.Address.AdditionalInfo = AddressAdditionalInfo;
            SetAddressLocation(seller, geocoder);

            seller.FacebookProfile = FacebookProfile;
            seller.LinkedInProfile = LinkedInProfile;
            seller.InstagramProfile = InstagramProfile;
            seller.GooglePlusProfile = GooglePlusProfile;
            seller.WhatsAppNumber = WhatsAppNumber;

            seller.PreferredTimeZoneId = PreferredTimeZoneId;

            friendlyUrlGenerator.Generate(seller);

            seller.CalculateScore();
            seller.ModifiedAt = DateTime.UtcNow;
        }

        private void SetAddressLocation(Seller seller, IGeocoder geocoder)
        {
            if (seller.Address == null)
                return;

            var hasLocation =
                !string.IsNullOrEmpty(AddressLat) && AddressLat != "0" &&
                !string.IsNullOrEmpty(AddressLng) && AddressLng != "0";

            if (hasLocation)
            {
                var parseCulture = AddressLat.Contains(",") ? CultureInfo.CreateSpecificCulture("pt-BR") : CultureInfo.InvariantCulture;

                using (new CultureContext(parseCulture))
                {
                    if (double.TryParse(AddressLat, out double lat) && double.TryParse(AddressLng, out double lng))
                    {
                        seller.Address.SetLocation(lat, lng);
                        return;
                    }
                }
            }

            geocoder.UpdateLocation(seller.Address);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateOfBirth.HasValue)
            {
                var config = MaaSConfig.Current;

                int age = DateTime.Today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value > DateTime.Today.AddYears(-age)) age--;

                if (age < config.Seller.DateOfBirth.MinAge)
                    yield return new ValidationResult(GlobalizationHelper.FormatString("SELLER_MIN_AGE_ERROR", config.Seller.DateOfBirth.MinAge), new[] { nameof(DateOfBirth) });

                if (age > config.Seller.DateOfBirth.MaxAge)
                    yield return new ValidationResult(GlobalizationHelper.FormatString("SELLER_MAX_AGE_ERROR", config.Seller.DateOfBirth.MaxAge), new[] { nameof(DateOfBirth) });
            }
        }
    }
}