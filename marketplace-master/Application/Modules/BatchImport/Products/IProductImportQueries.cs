using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Application.Modules.BatchImport.Products
{
    public interface IProductImportQueries
    {
        ReadOnlyCollection<KeyValuePair<string, Guid>> ListProductIdsByCustomIds(IEnumerable<string> customIds);
        ReadOnlyCollection<KeyValuePair<string, Guid>> ListProductIdsByCustomIds(IEnumerable<CustomIdAndSellerEmail> customIdsAndSellerEmails);
        ReadOnlyCollection<KeyValuePair<string, string>> ListSellerIdsBySellerEmails(IEnumerable<string> sellerEmails);
    }

    public struct CustomIdAndSellerEmail
    {
        public CustomIdAndSellerEmail(string customId, string sellerEmail)
        {
            CustomId = customId;
            SellerEmail = sellerEmail;
        }

        public string CustomId { get; }
        public string SellerEmail { get; }
    }
}
