using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTO
{
    public class ChangePasswordViewModel
    {
        [RequiredLocalized]
        [DataType(DataType.Password)]
        [DisplayNameLocalized("CURRENT_PASSWORD_LABEL")]
        public string OldPassword { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(100, "PASSWORD_LENGTH_ERROR", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayNameLocalized("NEW_PASSWORD_LABEL")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [DisplayNameLocalized("CONFIRM_NEW_PASSWORD_LABEL")]
        [CompareLocalized("NewPassword", "CONFIRM_NEW_PASSWORD_COMPARE_ERROR")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [RequiredLocalized]
        [EmailAddressLocalized]
        [DisplayNameLocalized("EMAIL_LABEL")]
        public string Email { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(100, "PASSWORD_LENGTH_ERROR", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayNameLocalized("NEW_PASSWORD_LABEL")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayNameLocalized("CONFIRM_NEW_PASSWORD_LABEL")]
        [CompareLocalized("Password", "CONFIRM_NEW_PASSWORD_COMPARE_ERROR")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [RequiredLocalized]
        [EmailAddressLocalized]
        [DisplayNameLocalized("EMAIL_LABEL")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [RequiredLocalized]
        [DisplayNameLocalized("EMAIL_LABEL")]
        [EmailAddressLocalized]
        public string Email { get; set; }

        [RequiredLocalized]
        [DataType(DataType.Password)]
        [DisplayNameLocalized("PASSWORD_LABEL")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
