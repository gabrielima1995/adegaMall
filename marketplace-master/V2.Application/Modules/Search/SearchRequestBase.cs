namespace V2.Application.Modules.Search
{
    public abstract class SearchRequestBase
    {
        public string Query { get; set; }

        public int StartIndex { get; set; }

        public int PageSize { get; set; }
    }
}
