using Application.Modules.BatchImport.Products;
using Domain.DTO;
using Domain.Interfaces;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.BatchImportAdapter.Products
{
    public class ProductImagesZipExtractor : IProductImagesZipExtractor
    {
        private readonly ICloudStorage cloudStorage;

        public ProductImagesZipExtractor(ICloudStorage cloudStorage)
        {
            this.cloudStorage = cloudStorage;
        }

        public Task<ReadOnlyCollection<string>> ReadFileNames(Stream stream)
        {
            ResetStream(stream);

            using (var zip = ZipFile.Read(stream))
            {
                var fileNames = zip.EntryFileNames
                    .Where(name => ImageHelper.IsSupportedType(name))
                    .OrderBy(name => name)
                    .ToList()
                    .AsReadOnly();

                return Task.FromResult(fileNames);
            }
        }

        public async Task<ReadOnlyDictionary<string, Uri>> SaveFilesToCloudStorage(Stream stream, IEnumerable<string> fileNames)
        {
            ResetStream(stream);
            var urls = new Dictionary<string, Uri>(fileNames.Count());

            using (var zip = ZipFile.Read(stream))
            {
                foreach (var name in fileNames)
                {
                    if (!ImageHelper.IsSupportedType(name))
                        continue;

                    using (var extractedFile = new MemoryStream())
                    {
                        zip[name].Extract(extractedFile);
                        var url = await SaveImage(extractedFile, name);
                        urls.Add(name, url);
                    }
                }
            }

            return new ReadOnlyDictionary<string, Uri>(urls);
        }

        private async Task<Uri> SaveImage(Stream fileStream, string fileName)
        {
            ResetStream(fileStream);
            var fileType = CloudFileType.ProductImage.ToString();
            var contentType = ImageHelper.GetContentType(fileName);
            return await cloudStorage.UploadFileAsync(fileType, fileName, contentType, fileStream);
        }

        private void ResetStream(Stream stream)
        {
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);
        }

        private static class ImageHelper
        {
            private static readonly string[] supported = new string[] { ".gif", ".jpg", ".jpeg", ".png" };

            public static bool IsSupportedType(string fileName)
            {
                return GetContentType(fileName) != null;
            }

            public static string GetContentType(string fileName)
            {
                foreach (var extension in supported)
                {
                    if (fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        var typeName = extension.Substring(1);
                        return "image/" + typeName;
                    }
                }

                return null;
            }
        }
    }
}
