using Domain.DTO;
using Domain.Interfaces;
using Framework;
using System;
using System.Collections.Generic;

namespace V2.Application.Helpers
{
    public class FileUploadHelper
    {
        private readonly ICloudStorage _cloudStorage;

        public FileUploadHelper(ICloudStorage cloudStorage)
        {
            _cloudStorage = cloudStorage;
        }

        public OperationResult<Uri> Upload(CloudFileType type, FileUpload fileUpload)
        {
            try
            {
                var uri = _cloudStorage.UploadFile(type.ToString(), fileUpload.FileName, fileUpload.ContentType, fileUpload.InputStream);
                return uri;
            }
            catch (Exception ex)
            {
                return new ErrorDictionary("", ex.Message);
            }
        }

        public OperationResult<List<Uri>> Upload(CloudFileType type, IEnumerable<FileUpload> fileUpload)
        {
            var list = new List<Uri>();

            foreach (var item in fileUpload)
            {
                var uploadResult = Upload(type, item);
                if (!uploadResult.OK) return uploadResult.Errors;

                list.Add(uploadResult.Result);
            }

            return list;
        }
    }
}
