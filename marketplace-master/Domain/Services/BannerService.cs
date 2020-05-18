using Domain.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Framework;
using Infra.Crosscutting.Globalization;
using System;
using System.Collections.Generic;

namespace Domain.Services
{
    public class BannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly ICloudStorage _cloudStorage;

        public BannerService(
            IBannerRepository bannerRepository,
            ICloudStorage cloudStorage)
        {
            _bannerRepository = bannerRepository;
            _cloudStorage = cloudStorage;
        }

        public Banner GetById(Guid id)
        {
            return _bannerRepository.GetById(id);
        }

        public List<Banner> ListActive()
        {
            return _bannerRepository.ListActive();
        }

        public List<Banner> ListForAdmin()
        {
            return _bannerRepository.ListForAdmin();
        }

        public OperationResult Create(Banner banner, FileUpload bannerImage)
        {
            return CreateUpdateInternal(banner, bannerImage, isUpdate: false);
        }

        public OperationResult Update(Banner banner, FileUpload bannerImage)
        {
            return CreateUpdateInternal(banner, bannerImage, isUpdate: true);
        }

        public OperationResult Delete(Banner banner)
        {
            banner.Deleted = true;
            banner.DeletedAt = DateTime.UtcNow;
            return _bannerRepository.Update(banner, saveChanges: true);
        }

        public Banner PrepareNew()
        {
            return new Banner();
        }

        public OperationResult ValidateForCreate(Banner banner, FileUpload bannerImage)
        {
            return ValidateForCreateUpdateInternal(banner, bannerImage, false);
        }

        public OperationResult ValidateForUpdate(Banner banner, FileUpload bannerImage)
        {
            return ValidateForCreateUpdateInternal(banner, bannerImage, true);
        }

        private OperationResult CreateUpdateInternal(Banner banner, FileUpload bannerImage, bool isUpdate)
        {
            Sanitize(banner);

            var validation = ValidateForCreateUpdateInternal(banner, bannerImage, isUpdate);
            if (!validation.OK) return validation;

            if (bannerImage != null && bannerImage.ContentLength > 0)
            {
                try
                {
                    var uri = _cloudStorage.UploadFile(CloudFileType.BannerImage.ToString(), bannerImage.FileName, bannerImage.ContentType, bannerImage.InputStream);
                    banner.ImageUrl = uri.ToString();
                }
                catch (Exception ex)
                {
                    return OperationResult.Fail("", ex.Message);
                }
            }

            if (isUpdate)
            {
                var updateResult = _bannerRepository.Update(banner, saveChanges: true);
                if (!updateResult.OK) return updateResult;
            }
            else
            {
                var addResult = _bannerRepository.Add(banner, saveChanges: true);
                if (!addResult.OK) return addResult;
            }

            return new OperationResult();
        }

        private void Sanitize(Banner banner)
        {
            StringHelper.Sanitize(banner);
        }

        private OperationResult ValidateForCreateUpdateInternal(Banner banner, FileUpload bannerImage, bool isUpdate)
        {
            var errors = new ErrorDictionary();

            if (banner == null)
            {
                errors.Add(nameof(banner), GlobalizationHelper.FormatString("REQUIRED_ERROR", nameof(banner)));
            }
            else
            {
                if (banner.Id == Guid.Empty)
                    errors.Add(nameof(banner.Id), GlobalizationHelper.FormatString("REQUIRED_ERROR", nameof(banner.Id)));

                if (banner.ActionUrl == null)
                    errors.Add(nameof(banner.ActionUrl), GlobalizationHelper.FormatString("REQUIRED_ERROR", nameof(banner.ActionUrl)));

                if (banner.ImageUrl == null && bannerImage.ContentLength == 0)
                    errors.Add(nameof(banner.ImageUrl), GlobalizationHelper.FormatString("REQUIRED_ERROR", nameof(banner.ImageUrl)));

                if (banner.ExpiresAt != null && banner.StartsAt != null && banner.ExpiresAt < banner.StartsAt)
                    errors.Add(nameof(banner.StartsAt), GlobalizationHelper.GetString("BANNER_EXPIRES_BEFORE_START_ERROR"));
            }

            return errors;
        }
    }
}
