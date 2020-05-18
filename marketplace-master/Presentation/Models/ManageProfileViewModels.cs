using Application.Helpers;
using Domain.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using V2.Application.Modules.UserAccounts;

namespace Presentation.Models
{
    public class ManageProfileViewModel
    {
        public ManageProfileViewModel() { }
        public ManageProfileViewModel(ListProfilesForUserResult profiles)
        {
            Profiles = profiles.Profiles.Select(profile => new ManageProfileItemViewModel(profile)).ToReadOnly();
        }

        public ReadOnlyCollection<ManageProfileItemViewModel> Profiles { get; } = new List<ManageProfileItemViewModel>().AsReadOnly();
        public string LoginWithPersonId { get; set; }
        public bool CanRegisterAsBuyer => !Profiles.Any(profile => profile.Type == PersonType.Buyer);
        public bool CanRegisterAsSeller => !Profiles.Any(profile => profile.Type == PersonType.Seller);
    }

    public class ManageProfileItemViewModel
    {
        public ManageProfileItemViewModel() { }
        public ManageProfileItemViewModel(ListProfilesForUserResultItem profile)
        {
            UserId = profile.UserId;
            PersonId = profile.PersonId;
            Avatar = ImgHelper.Avatar(profile.Avatar, 300);
            Name = profile.Name;
            DocumentNumber = profile.DocumentNumber;
            IsDisabled = profile.IsDisabled;
            Type = profile.Type;
        }

        public string UserId { get; set; }
        public string PersonId { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string DocumentNumber { get; set; }
        public bool IsDisabled { get; set; }
        public PersonType Type { get; set; }
    }
}