using Application.Modules.BatchImport.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Modules.BatchImport.Products
{
    public class ImportProductImagesCommand
    {
        public ImportProductImagesCommand(Stream zipStream)
        {
            ZipStream = zipStream;
        }

        public Stream ZipStream { get; }
    }

    public class ImportProductImagesCommandHandler
    {
        private readonly IProductImagesZipExtractor zipExtractor;
        private readonly IProductImportQueries queries;
        private readonly IProductImportRepository repository;

        public ImportProductImagesCommandHandler(
            IProductImagesZipExtractor zipExtractor,
            IProductImportQueries queries,
            IProductImportRepository repository)
        {
            this.zipExtractor = zipExtractor;
            this.queries = queries;
            this.repository = repository;
        }

        public async Task<ImportResult<string>> HandleAsync(ImportProductImagesCommand command)
        {
            var fileNames = (await zipExtractor.ReadFileNames(command.ZipStream)).OrderBy(file => file).ToList();
            var imageFiles = fileNames.Select(name => ImageFile.FromFileName(name));

            imageFiles = AssingProductIdsByCustomIds(imageFiles);
            imageFiles = await UploadAndAssignUrlAsync(command.ZipStream, imageFiles);

            var productIdsAndUrls = imageFiles.Select(image => image.AsProductIdAndImageUrl());
            await repository.InsertProductImageRecordsAsync(productIdsAndUrls);
            await repository.FixProductCoverImageChangingItFromDefaultAsync(productIdsAndUrls);

            var uploadedFiles = imageFiles.Select(image => image.FileName);
            var failedFiles = fileNames.Except(uploadedFiles);

            return BuildImportResult(uploadedFiles, failedFiles);
        }

        private ImportResult<string> BuildImportResult(IEnumerable<string> uploadedFiles, IEnumerable<string> failedFiles)
        {
            var errorMessage = "Não encontramos um produto com o Id Personalizado inferido a partir do nome do arquivo.";

            return new ImportResult<string>(
                new[] { "FileName" },
                uploadedFiles,
                failedFiles.Select(file => new InvalidImportImage(new[] { InvalidImportField.InvalidField("FileName", file, errorMessage) })));
        }

        private IEnumerable<ImageFile> AssingProductIdsByCustomIds(IEnumerable<ImageFile> imageFiles)
        {
            var customIdRoots = imageFiles.Select(image => image.CustomIdRoot).Distinct();
            var customIdsToProductIds = queries.ListProductIdsByCustomIds(customIdRoots);
            var imagesWithProductIds = new List<ImageFile>(imageFiles.Count());

            foreach (var entry in customIdsToProductIds)
            {
                var customIdRoot = entry.Key;
                var relatedImages = imageFiles.Where(image => image.CustomIdRoot == customIdRoot);

                foreach (var image in relatedImages)
                {
                    image.ProductId = entry.Value;
                    imagesWithProductIds.Add(image);
                }
            }

            return imagesWithProductIds;
        }

        private async Task<IEnumerable<ImageFile>> UploadAndAssignUrlAsync(Stream zipStream, IEnumerable<ImageFile> imageFiles)
        {
            var filesForUpload = imageFiles.Where(image => image.HasProductId).Select(image => image.FileName);
            var uploadedUrls = await zipExtractor.SaveFilesToCloudStorage(zipStream, filesForUpload);
            var imagesWithUrl = new List<ImageFile>(imageFiles.Count());

            foreach (var image in imageFiles)
            {
                if (uploadedUrls.ContainsKey(image.FileName))
                {
                    image.UploadedUrl = uploadedUrls[image.FileName];
                    imagesWithUrl.Add(image);
                }
            }

            return imagesWithUrl;
        }

        private class ImageFile
        {
            private ImageFile(string fileName, string customId, string customIdRoot)
            {
                FileName = fileName;
                CustomId = customId;
                CustomIdRoot = customIdRoot;
            }

            public static ImageFile FromFileName(string fileName)
            {
                var customId = Path.GetFileNameWithoutExtension(fileName);

                var root = customId;
                if (root.IndexOf('-') > 0)
                    root = root.Substring(0, root.IndexOf('-'));

                return new ImageFile(fileName, customId, root);
            }

            public string FileName { get; }
            public string CustomId { get; }
            public string CustomIdRoot { get; }
            public Guid? ProductId { get; set; }
            public Uri UploadedUrl { get; set; }

            public bool HasProductId => ProductId != null;

            public KeyValuePair<Guid, string> AsProductIdAndImageUrl()
            {
                if (ProductId == null)
                    throw new InvalidOperationException("Invalid call to AsProductIdAndImageUrl when the ImageFile instance has no ProductId.");

                return new KeyValuePair<Guid, string>(ProductId.Value, UploadedUrl.ToString());
            }
        }
    }
}
