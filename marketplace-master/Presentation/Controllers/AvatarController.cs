using Application.Avatar;
using Application.Config;
using Domain.DTO;
using Domain.Events;
using Domain.Events.SellerProfileUpdated;
using Domain.Interfaces.Repositories;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using V2.Application.Modules.SellerAssistantModule;

namespace Presentation.Controllers
{
    [AuthorizeBuyerOrSellerOrSellerAssistant]
    public class AvatarController : BaseController
    {
        private readonly AvatarAppService avatarAppService;
        private readonly IPersonRepository personRepository;

        public AvatarController(
            AvatarAppService avatarAppService,
            IPersonRepository personRepository)
        {
            this.avatarAppService = avatarAppService;
            this.personRepository = personRepository;
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.ProfileEdit)]
        public async Task<ActionResult> Change(HttpPostedFileBase file)
        {
            var profile = GetLoginProfile();
            if (profile.IsSellerOrSellerAssistant() && !MaaSConfig.Current.Seller.ChangeAvatar)
                return new EmptyResult();

            var personId = GetPersonId();
            await avatarAppService.ChangeAvatarAsync(personId, file);

            return new EmptyResult();
        }

        [HttpPost]
        [RequireAssistantPermissions(SellerAssistantPermissions.ProfileEdit)]
        public ActionResult Remove()
        {
            var personId = GetPersonId();
            var person = personRepository.GetById(personId);

            if (person == null)
                return new HttpNotFoundResult();

            var profile = GetLoginProfile();
            if (profile.IsSellerOrSellerAssistant() && !MaaSConfig.Current.Seller.ChangeAvatar)
                return new EmptyResult();

            person.Avatar = null;
            person.AddDomainEvent(new PersonUpdatedEvent(person.Id, profile.IsBuyer(), profile.IsSellerOrSellerAssistant()));

            if (profile.IsSellerOrSellerAssistant())
                person.AddDomainEvent(new SellerProfileUpdated(person.Id));

            personRepository.Update(person);
            personRepository.SaveChanges();

            return new EmptyResult();
        }
    }
}