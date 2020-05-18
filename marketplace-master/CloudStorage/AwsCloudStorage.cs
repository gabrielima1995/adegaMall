using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Application.Config;
using Domain.Interfaces;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CloudStorage
{
    public class AwsCloudStorage : ICloudStorage
    {
        private readonly RegionEndpoint _regionEndpoint = RegionEndpoint.USEast1;

        private string _accessKey = null;
        private string _bucketName = null;
        private string _secretKey = null;


        public string AccessKey
        {
            get
            {
                if (_accessKey == null)
                    _accessKey = MaaSConfig.Current.Environment.CloudStorage.Username;

                return _accessKey;
            }
            set
            {
                _accessKey = value;
            }
        }
        public string BucketName
        {
            get
            {
                if (_bucketName == null)
                {
                    var name = new Uri(MaaSConfig.Current.Environment.CloudStorage.Endpoint).Host.Split('.')[0];
                    _bucketName = name;
                }

                return _bucketName;
            }
            set
            {
                _bucketName = value;
            }
        }
        public string SecretKey
        {
            get
            {
                if (_secretKey == null)
                    _secretKey = MaaSConfig.Current.Environment.CloudStorage.Password;

                return _secretKey;
            }
            set
            {
                _secretKey = value;
            }
        }

        public AwsCloudStorage() { }

        public Uri UploadFile(string fileType, string fileName, string contentType, Stream stream)
        {
            var key = NameHelper.NormalizeContainerName(fileType) + "/" + NameHelper.NormalizeBlobName(fileName);
            var utility = new TransferUtility(AccessKey, SecretKey, _regionEndpoint);

            utility.Upload(new TransferUtilityUploadRequest
            {
                BucketName = BucketName,
                Key = key,
                InputStream = stream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            });

            return GetUri(key);
        }

        public async Task<Uri> UploadFileAsync(string fileType, string fileName, string contentType, Stream stream)
        {
            var key = NameHelper.NormalizeContainerName(fileType) + "/" + NameHelper.NormalizeBlobName(fileName);
            var utility = new TransferUtility(AccessKey, SecretKey, _regionEndpoint);

            await utility.UploadAsync(new TransferUtilityUploadRequest
            {
                BucketName = BucketName,
                Key = key,
                InputStream = stream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            });

            return GetUri(key);
        }

        public Uri UploadFileFromUri(string fileType, string fileName, Uri source)
        {
            var request = WebRequest.Create(source);

            using (var response = request.GetResponse())
            {
                var contentType = response.ContentType;
                var stream = response.GetResponseStream();

                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    ms.Position = 0;
                    return UploadFile(fileType, fileName, contentType, ms);
                }
            }
        }

        public async Task<Uri> UploadFileFromUriAsync(string fileType, string fileName, Uri source)
        {
            var request = WebRequest.Create(source);

            using (var response = await request.GetResponseAsync())
            {
                var contentType = response.ContentType;
                var stream = response.GetResponseStream();

                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    ms.Position = 0;
                    return await UploadFileAsync(fileType, fileName, contentType, ms);
                }
            }
        }

        public void DoSetup()
        {
            var utility = new TransferUtility(AccessKey, SecretKey, _regionEndpoint);

            if (!utility.S3Client.DoesS3BucketExist(BucketName))
            {
                var request = new PutBucketRequest { BucketName = BucketName };
                utility.S3Client.PutBucket(request);
            }
        }

        private Uri GetUri(string resource)
        {
            return new Uri($"https://{BucketName}.s3.amazonaws.com/{resource}");
        }
    }
}
