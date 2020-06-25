using Domain;
using Domain.DTO;
using Domain.Entities;
using Domain.Entities.PortfolioModule;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Framework;
using Presentation;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.AspNet.Identity
{
    public static class IdentityExtensions
    {
        public static string GetMyAccountHandler(this IIdentity identity)
        {
            var person = GetLoginProfile(identity);
            if (person == null) return null;

            var input = person.Email ?? person.Name;
            var index = input.IndexOf('@');

            return "@" + (index == -1 ? input : input.Substring(0, index));
        }

        public static LoginProfile GetLoginProfile(this IIdentity identity)
        {
            var userAuth = UnityConfig.Resolve<IUserAuthProvider>();
            var personRepository = UnityConfig.Resolve<IPersonRepository>();
            return personRepository.GetLoginProfile(userAuth.UserId, userAuth.PersonId) ?? new LoginProfile();
        }

        public static string GetPersonId(this IIdentity identity)
        {
            return identity.GetLoginProfile().PersonId;
        }

        public static int GetTotalOpenedQuotes(this IIdentity identity)
        {
            var sellerId = identity.GetPersonId();
            return UnityConfig.Resolve<IRepository<Quote>>().Query()
                .Count(i => i.Seller.Id == sellerId && i.Status == QuoteStatus.Opened);
        }
    }
}