using Domain.DTO;
using Domain.Interfaces;
using Framework;
using System;

namespace Domain.Services
{
    public class UploadService
    {
        private readonly ICloudStorage _cloudStorage;

        public UploadService(ICloudStorage cloudStorage)
        {
            _cloudStorage = cloudStorage;
        }

        public OperationResult<Uri> Upload(CloudFileType fileType, FileUpload fileUpload)
        {
            var validationResult = ValidateForUpload(fileType, fileUpload);
            if (!validationResult.OK) return validationResult.Errors;

            try
            {
                var uri = _cloudStorage.UploadFile(fileType.ToString(), fileUpload.FileName, fileUpload.ContentType, fileUpload.InputStream);
                return uri;
            }
            catch (Exception ex)
            {
                return new ErrorDictionary("", ex.Message);
            }
        }

        private OperationResult ValidateForUpload(CloudFileType fileType, FileUpload fileUpload)
        {
            var errors = new ErrorDictionary();

            if (fileUpload == null)
            {
                errors.Add(nameof(fileUpload), "File upload cannot be null");
            }
            else
            {
                if (fileUpload.ContentLength == 0)
                    errors.Add(nameof(fileUpload.ContentLength), "The file cannot be empty.");

                if (fileUpload.FileName == null)
                    errors.Add(nameof(fileUpload.FileName), "The file name cannot be empty.");

                if (fileUpload.InputStream == null)
                    errors.Add(nameof(fileUpload.InputStream), "The file stream cannot be null.");
            }

            return errors;
        }
    }
}
