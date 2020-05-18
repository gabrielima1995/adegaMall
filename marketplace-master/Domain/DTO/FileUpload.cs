using System;
using System.IO;

namespace Domain.DTO
{
    public class FileUpload : IDisposable
    {
        public string FileName { get; private set; }

        public string ContentType { get; private set; }

        public Stream InputStream { get; private set; }

        public double ContentLength => InputStream?.Length ?? 0;

        public FileUpload()
        {
        }

        public FileUpload(Stream inputStream, string fileName, string contentType)
        {
            InputStream = inputStream;
            FileName = fileName;
            ContentType = contentType;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (InputStream != null) InputStream.Dispose();
            }
        }
    }
}
