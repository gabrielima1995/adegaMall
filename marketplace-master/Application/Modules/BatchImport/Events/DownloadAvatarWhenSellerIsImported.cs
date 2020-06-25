using Domain.DTO;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace Application.Modules.BatchImport.Events
{
    public class DownloadAvatarWhenSellerIsImported : IDomainEventHandler<SellerImportedEvent>
    {
        private readonly ICloudStorage _cloudStorage;
        private readonly ISocialImageProviderResolver _imageProviderResolver;
        private readonly ISellerAvatarUpdater _avatarUpdater;

        public DownloadAvatarWhenSellerIsImported(ICloudStorage cloudStorage, ISocialImageProviderResolver imageProviderResolver, ISellerAvatarUpdater avatarUpdater)
        {
            _cloudStorage = cloudStorage;
            _imageProviderResolver = imageProviderResolver;
            _avatarUpdater = avatarUpdater;
        }

        public void Handle(SellerImportedEvent domainEvent)
        {
            string avatarUrl;

            if (TryDownloadAndSaveImage(domainEvent.Seller, out avatarUrl))
                _avatarUpdater.UpdateSellerAvatar(domainEvent.Seller.Id, avatarUrl);
        }

        private IEnumerable<string> GetSocialProfileUrls(Seller seller)
        {
            if (seller.FacebookProfile != null)
                yield return seller.FacebookProfile;

            if (seller.LinkedInProfile != null)
                yield return seller.LinkedInProfile;

            if (seller.InstagramProfile != null)
                yield return seller.InstagramProfile;

            if (seller.GooglePlusProfile != null)
                yield return seller.GooglePlusProfile;
        }

        private bool TryDownloadAndSaveImage(Seller seller, out string avatarUrl)
        {
            var socialUrls = GetSocialProfileUrls(seller);
            avatarUrl = null;

            foreach (var url in socialUrls)
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    continue;

                foreach (var provider in _imageProviderResolver.ResolveAll())
                {
                    if (!provider.CanHandleUrl(url))
                        continue;

                    var imageBytes = provider.GetImage(url);
                    if (imageBytes == null)
                        continue;

                    var fileName = $"{seller.Id.Substring(0, 8)}-{seller.Name}.jpg";

                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var fileType = CloudFileType.UserImage.ToString();
                        avatarUrl = _cloudStorage.UploadFile(fileType, fileName, "image/jpeg", stream).ToString();
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public interface ISocialImageProviderResolver
    {
        IEnumerable<ISocialImageProvider> ResolveAll();
    }
}
