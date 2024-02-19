namespace ROA.Utilities.Models;

public class PagedSearchCursorList<T>: PagedCursorList<T> 
{
    public PagedSearchCursorList(PagedCursorList<T> pagedCursorList, string currentSearch,  string nextSearch, int totalSearchItems)
        :this(pagedCursorList.Items, pagedCursorList.TotalItems, pagedCursorList.LastId, currentSearch, nextSearch, totalSearchItems)
    {

    }
    
    public PagedSearchCursorList(IEnumerable<T> items, int totalItems, string lastId, string currentSearch, string nextSearch, int totalSearchItems)
        :base(items, totalItems, lastId)
    {
        CurrentSearch = currentSearch;
        NextSearch = nextSearch;
        TotalSearchItems = totalSearchItems;
    }
    
    public string CurrentSearch { get; set; }
    
    public string NextSearch { get; set; }
    
    public int TotalSearchItems { get; set; }

    public static PagedSearchCursorList<T> CreateEmpty()
    {
        return new PagedSearchCursorList<T>(Enumerable.Empty<T>(), 0, null, null, null, 0);
    }
}