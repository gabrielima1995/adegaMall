using Application.Config;
using Domain.Entities;
using Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Application.Sellers
{
    public class SellerRegisterViewModel
    {
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

        public bool PhoneNumberRequired => MaaSConfig.Current.Seller.Phone.IsRequired;

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

        public SellerUserAndProfile GetUserAndProfile()
        {
            var userAndProfile = SellerUserAndProfile.Create(UserOrPersonId.Generate(), Email,
                requireModeration: MaaSConfig.Current.Seller.Moderation.Enabled);

            var seller = userAndProfile.Profile;
            seller.FullName = Name;
            seller.Nickname = Nickname;
            seller.PhoneNumber = PhoneNumber;
            seller.AlternativeName = Name;
            seller.Lang = Lang;

            return userAndProfile;
        }
    }
}