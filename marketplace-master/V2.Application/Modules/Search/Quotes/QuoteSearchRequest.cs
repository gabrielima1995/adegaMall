using System;

namespace V2.Application.Modules.Search.Quotes
{
    public class QuoteSearchRequest : SearchRequestBase
    {
        public QuoteSort Sort = QuoteSort.MostRelevant;

        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public int? Category { get; set; }
        public int? SubCategory { get; set; }
    }
}