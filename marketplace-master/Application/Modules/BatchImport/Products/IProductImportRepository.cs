using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Modules.BatchImport.Products
{
    public interface IProductImportRepository
    {
        Task FixProductCoverImageChangingItFromDefaultAsync(IEnumerable<KeyValuePair<Guid, string>> productIdsAndImageUrls);
        Task InsertProductImageRecordsAsync(IEnumerable<KeyValuePair<Guid, string>> productIdsAndImageUrls);
    }
}
