using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Application.Modules.BatchImport.Products
{
    public interface IProductImagesZipExtractor
    {
        Task<ReadOnlyCollection<string>> ReadFileNames(Stream stream);
        Task<ReadOnlyDictionary<string, Uri>> SaveFilesToCloudStorage(Stream stream, IEnumerable<string> fileNames);
    }
}
