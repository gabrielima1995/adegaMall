using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using V2.Application.Modules.UserAccounts;

namespace Presentation.Models
{
    public class AdditionalProfileViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string OriginalPersonId { get; set; }

        [Required]
        public PersonType NewProfileType { get; set; }

        public RegisterAdditionalProfile AsRequest() => new RegisterAdditionalProfile(UserId, OriginalPersonId, NewProfileType);
    }
}