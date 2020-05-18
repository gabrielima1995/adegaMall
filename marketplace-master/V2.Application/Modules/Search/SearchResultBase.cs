namespace V2.Application.Modules.Search
{
    public abstract class SearchResultBase
    {
        public int TotalCount { get; set; }
    }

    public abstract class SearchResultBase<T> : SearchResultBase
    {
        public T[] Items { get; set; }
    }
}
