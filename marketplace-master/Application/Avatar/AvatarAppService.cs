using Domain.DTO;
using Domain.Events;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Application.Config;
using Domain.Events.SellerProfileUpdated;
using Domain.Entities;

namespace Application.Avatar
{
    public class AvatarAppService
    {
        private readonly ICloudStorage cloudStorage;
        private readonly IPersonRepository personRepository;

        public AvatarAppService(
            ICloudStorage cloudStorage,
            IPersonRepository personRepository)
        {
            this.cloudStorage = cloudStorage;
            this.personRepository = personRepository;
        }

        public async Task ChangeAvatarAsync(string personId, HttpPostedFileBase avatarFile)
        {
            if (avatarFile == null && avatarFile.ContentLength == 0) return;
            if (!IsSupportedExtension(avatarFile.FileName)) return;

            var person = personRepository.GetById(personId);
            if (person == null) return;

            var isBuyer = person.PersonType == PersonType.Buyer;
            var isSeller = person.PersonType == PersonType.Seller;
            if (isSeller && !MaaSConfig.Current.Seller.ChangeAvatar) return;

            var uri = await cloudStorage.UploadFileAsync(
                CloudFileType.UserImage.ToString(),
                avatarFile.FileName, avatarFile.ContentType, avatarFile.InputStream);

            person.Avatar = uri.ToString();
            person.AddDomainEvent(new PersonUpdatedEvent(personId, isBuyer, isSeller));
            if (isSeller) person.AddDomainEvent(new SellerProfileUpdated(personId));

            personRepository.Update(person);
            personRepository.SaveChanges();
        }

        private bool IsSupportedExtension(string fileName)
        {
            var supported = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
            return supported.Any(extensions => fileName.EndsWith(extensions, StringComparison.OrdinalIgnoreCase));
        }
    }
}