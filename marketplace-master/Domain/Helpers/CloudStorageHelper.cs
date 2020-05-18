using Application.Config;
using Domain.DTO;
using System;
using System.IO;

namespace Domain.Helpers
{
    public static class CloudStorageHelper
    {
        public static string GetContainerName(this CloudFileType type)
        {
            return type.ToString().ToLower();
        }

        public static string CreateCloudUrl(CloudFileType type, FileUpload postedFile)
        {
            return CreateCloudUrl(type, postedFile.FileName);
        }

        public static string CreateCloudUrl(CloudFileType type, string sourceFileName)
        {
            var path = GetPath(type, sourceFileName);
            var baseUri = new Uri(MaaSConfig.Current.Environment.CloudStorage.Endpoint);
            var cloudUri = new Uri(baseUri, path);
            return cloudUri.ToString();
        }

        private static string GetPath(CloudFileType type, string sourceFileName)
        {
            var containerName = type.GetContainerName();
            var fileName = GetFileName(sourceFileName);
            var path = Path.Combine(containerName, fileName);
            return path.Replace("\\", "/");
        }

        private static string GetFileName(string input)
        {
            var guid = Guid.NewGuid().ToString().Substring(0, 8);
            var fileName = Path.GetFileNameWithoutExtension(input);
            var extension = Path.GetExtension(input);
            var hyphenized = StringHelper.Hyphenize(fileName);
            var result = $"{guid}-{hyphenized}{extension}".ToLower();
            return result;
        }
    }
}