using Domain.Entities;
using MediatR;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace V2.Application.Modules.UserAccounts
{
    public class ListProfilesForUser : IRequest<ListProfilesForUserResult>
    {
        public ListProfilesForUser(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }

    public class ListProfilesForUserResult
    {
        public ListProfilesForUserResult(IEnumerable<ListProfilesForUserResultItem> profiles)
        {
            Profiles = profiles.ToList().AsReadOnly();
        }

        public ReadOnlyCollection<ListProfilesForUserResultItem> Profiles { get; }
    }

    public class ListProfilesForUserResultItem
    {
        public string UserId { get; set; }
        public string PersonId { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string DocumentNumber { get; set; }
        public bool IsDisabled { get; set; }
        public PersonType Type { get; set; }
    }
}
