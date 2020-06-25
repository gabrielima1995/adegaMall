using Domain.Entities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Areas.Admin.Models
{
    public class AdminCreateViewModel
    {
        [DisplayNameLocalized("COMPLETE_NAME_LABEL")]
        [RequiredLocalized]
        [StringLengthLocalized(1024)]
        public string Name { get; set; }

        [DisplayNameLocalized("EMAIL_LABEL")]
        [RequiredLocalized]
        [EmailAddressLocalized]
        [StringLengthLocalized(100)]
        public string Email { get; set; }

        [DisplayNameLocalized("PASSWORD_INPUT")]
        [RequiredLocalized]
        [DataType(DataType.Password)]
        [StringLengthLocalized(100, "PASSWORD_LENGTH_ERROR", MinimumLength = 6)]
        public string Password { get; set; }

        [DisplayNameLocalized("CONFIRM_PASSWORD_INPUT")]
        [DataType(DataType.Password)]
        [CompareLocalized("Password", "CONFIRM_PASSWORD_COMPARE_ERROR")]
        public string ConfirmPassword { get; set; }

        public AdminUserAndProfile Create()
        {
            return AdminUserAndProfile.Create(Framework.UserOrPersonId.Generate(), Email, Name);
        }
    }
}