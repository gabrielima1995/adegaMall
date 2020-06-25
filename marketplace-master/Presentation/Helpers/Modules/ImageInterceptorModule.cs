using ImageResizer;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;

namespace Presentation.Helpers.Modules
{
    public class ImageInterceptorModule : IHttpModule
    {
        private static readonly object Lock = new object();

        private HttpApplication _application;
        private HttpContext _context;

        private static string _cloudBaseUrl;
        public static string CloudBaseUrl
        {
            get => _cloudBaseUrl;
            set
            {
                if (value != null) value = value.TrimEnd('/') + '/';
                _cloudBaseUrl = value;
            }
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Application_BeginRequest;
        }

        private void AddResponseHeaders(PreProcessingResults result)
        {
            var lastModified = new FileInfo(result.PhysicalPath).LastWriteTimeUtc;

            _context.Response.ContentType = "image/jpeg";
            _context.Response.CacheControl = "Public";
            _context.Response.Cache.SetMaxAge(new TimeSpan(365, 0, 0));
            _context.Response.Cache.SetETag(GetChecksum(result.PhysicalPath));
            _context.Response.Cache.SetLastModified(lastModified);
        }

        private void Application_BeginRequest(object source, EventArgs e)
        {
            _application = (HttpApplication)source;
            _context = _application.Context;

            var result = PreProcessRequest();

            if (!result.ShouldHandleRequest)
                return;

            if (!result.ExistsPhysically)
                DownloadFile(result);

#if !DEBUG
            SetResponseCompression();
#endif
            AddResponseHeaders(result);
            ProcessImageAndSend(result);
        }

        private void CreateFolderStructure(PreProcessingResults result)
        {
            var dir = Path.GetDirectoryName(result.PhysicalPath);

            lock (Lock)
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
        }

        private void DownloadFile(PreProcessingResults result)
        {
            CreateFolderStructure(result);
            var request = WebRequest.Create(GetCloudImageUrl());
            using (var response = request.GetResponse())
            {
                using (var fileStream = File.OpenWrite(result.PhysicalPath))
                {
                    var sourceStream = response.GetResponseStream();
                    sourceStream.CopyTo(fileStream);
                    result.ExistsPhysically = File.Exists(result.PhysicalPath);
                }
            }
        }

        private string GetCloudImageUrl()
        {
            var parts = _context.Request.Url.LocalPath
                .Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            string path = null;

            if (parts.Count > 1)
            {
                parts.RemoveAt(0);
                path = string.Join("/", parts);
            }

            return _cloudBaseUrl + path;
        }

        private string GetChecksum(string filePath)
        {
            // https://stackoverflow.com/questions/1177607/what-is-the-fastest-way-to-create-a-checksum-for-large-files-in-c-sharp

            Exception latestException = null;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 16 * 1024 * 1024))
                    {
                        var sha = new SHA256Managed();
                        var checksum = sha.ComputeHash(stream);
                        return BitConverter.ToString(checksum).Replace("-", string.Empty);
                    }
                }
                catch (IOException ex)
                {
                    latestException = ex;
                    System.Threading.Thread.Sleep((i + 1) * 50);
                }
            }

            if (latestException != null)
                throw latestException;

            return null;
        }

        private PreProcessingResults PreProcessRequest()
        {
            var results = new PreProcessingResults();

            if (ShouldProcessRequest())
            {
                results.ShouldHandleRequest = true;
                results.VirtualPath = _context.Request.Url.LocalPath;
                results.PhysicalPath = _application.Server.MapPath(results.VirtualPath);
                results.ExistsPhysically = File.Exists(results.PhysicalPath);
            }

            return results;
        }

        private void ProcessImageAndSend(PreProcessingResults result)
        {
            var instructions = new Instructions(_context.Request.QueryString)
            {
                AutoRotate = true,
                JpegQuality = 75
            };

            for (var tryCount = 0; tryCount < 3; tryCount++)
            {
                try
                {
                    ImageBuilder.Current.Build(
                        result.PhysicalPath,
                        _context.Response.OutputStream,
                        instructions);

                    break;
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(500);
                    continue;
                }
            }

            _context.Response.End();
        }

        private void SetResponseCompression()
        {
            string encodings = _context.Request.Headers.Get("Accept-Encoding");

            if (encodings != null)
            {
                encodings = encodings.ToLower();

                if (encodings.Contains("deflate"))
                {
                    _context.Response.Filter = new DeflateStream(_context.Response.Filter, CompressionMode.Compress);
                    _context.Response.AppendHeader("Content-Encoding", "deflate");
                }
                else if (encodings.Contains("gzip"))
                {
                    _context.Response.Filter = new GZipStream(_context.Response.Filter, CompressionMode.Compress);
                    _context.Response.AppendHeader("Content-Encoding", "gzip");
                }
            }
        }

        private bool ShouldProcessRequest()
        {
            if (_context.Request.HttpMethod != "GET")
                return false;

            if (_context.Request.Url.Segments.Length < 2)
                return false;

            var prefix = _context.Request.Url.LocalPath
                .Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();

            if (!prefix.Equals(ConfigHelper.GetSetting("ImageInterceptor:Prefix", "cloudimage"), StringComparison.OrdinalIgnoreCase))
                return false;

            var extension = Path.GetExtension(_context.Request.Url.LocalPath).ToLower();

            if (extension.Length == 0 || !".bmp.gif.jpg.jpeg.png".Contains(extension))
                return false;

            return true;
        }

        public void Dispose()
        {
        }

        // Nested types.
        private class PreProcessingResults
        {
            public bool ShouldHandleRequest { get; set; }
            public string PhysicalPath { get; set; }
            public string VirtualPath { get; set; }
            public bool ExistsPhysically { get; set; }
        }
    }
}