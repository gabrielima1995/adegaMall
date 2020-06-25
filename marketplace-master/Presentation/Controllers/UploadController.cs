using Domain.DTO;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Presentation.Controllers
{
    [AuthorizeOnlyEnabled]
    [RoutePrefix("api/upload")]
    public class UploadController : ApiController
    {
        private readonly ICloudStorage _cloudStorage;

        public UploadController(ICloudStorage cloudStorage)
        {
            _cloudStorage = cloudStorage;
        }

        [HttpPost]
        [Obsolete("Use um dos métodos específicos como PostProductImage, PostPortfolioImage e PostUserImage.")]
        public async Task<IHttpActionResult> Post()
        {
            return await PostImage(CloudFileType.PlatformImage);
        }

        [HttpPost]
        [Route("productimage")]
        public async Task<IHttpActionResult> ProductImage()
        {
            return await PostImage(CloudFileType.ProductImage);
        }

        [HttpPost]
        [Route("producttemplateimage")]
        public async Task<IHttpActionResult> ProductTemplateImage()
        {
            return await PostImage(CloudFileType.ProductTemplateImage);
        }

        [HttpPost]
        [Route("portfolioimage")]
        public async Task<IHttpActionResult> PortfolioImage()
        {
            return await PostImage(CloudFileType.PortfolioImage);
        }

        [HttpPost]
        [Route("userimage")]
        public async Task<IHttpActionResult> UserImage()
        {
            return await PostImage(CloudFileType.UserImage);
        }

        private async Task<IHttpActionResult> PostImage(CloudFileType cloudFileType)
        {
            if (!Request.Content.IsMimeMultipartContent())
                return StatusCode(HttpStatusCode.UnsupportedMediaType);

            try
            {
                var files = new List<string>();
                var provider = await GetMultipartStreamProviderAsync();

                foreach (var content in provider.Contents)
                {
                    var stream = await content.ReadAsStreamAsync();
                    var containerName = cloudFileType.ToString();
                    var blobName = content.Headers.ContentDisposition?.FileName;
                    var contentType = content.Headers.ContentType.MediaType;

                    var uri = _cloudStorage.UploadFile(containerName, blobName, contentType, stream);
                    files.Add(uri.ToString());
                }

                return Ok(files);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private async Task<MultipartStreamProvider> GetMultipartStreamProviderAsync()
        {
            // Unexpected end of MIME multipart stream. MIME multipart message is not complete
            // https://stackoverflow.com/a/17290999/484108

            var provider = new MultipartMemoryStreamProvider();

            var reqStream = Request.Content.ReadAsStreamAsync().Result;
            var tempStream = new MemoryStream();
            reqStream.CopyTo(tempStream);

            tempStream.Seek(0, SeekOrigin.End);
            var writer = new StreamWriter(tempStream);
            writer.WriteLine();
            writer.Flush();
            tempStream.Position = 0;

            var streamContent = new StreamContent(tempStream);
            foreach (var header in Request.Content.Headers)
                streamContent.Headers.Add(header.Key, header.Value);

            await streamContent.ReadAsMultipartAsync(provider);
            return provider;
        }
    }
}