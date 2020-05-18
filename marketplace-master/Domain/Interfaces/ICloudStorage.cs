using System;
using System.IO;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICloudStorage
    {
        Uri UploadFile(string fileType, string fileName, string contentType, Stream stream);

        Uri UploadFileFromUri(string fileType, string fileName, Uri source);

        Task<Uri> UploadFileAsync(string fileType, string fileName, string contentType, Stream stream);

        Task<Uri> UploadFileFromUriAsync(string fileType, string fileName, Uri source);

        void DoSetup();
    }
}