namespace OpenBooksBackMobile.Shared
{
    public class PagedResult<T>
    {
        public List<T> Results { get; set; }
        public int TotalRecords { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
    }
}
