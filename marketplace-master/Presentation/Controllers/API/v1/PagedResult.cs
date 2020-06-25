using System.Collections.Generic;

namespace Presentation.Controllers.API.v1
{
    public class PagedResult
    {
        public PagedResult(IEnumerable<object> data, int totalItems, string previousPageUrl, string nextPageUrl)
        {
            Data = data;
            TotalItems = totalItems;
            NextPage = nextPageUrl;
            PrevPage = previousPageUrl;
        }

        public IEnumerable<object> Data { get; }
        public int TotalItems { get; }
        public string PrevPage { get; }
        public string NextPage { get; }
    }
}