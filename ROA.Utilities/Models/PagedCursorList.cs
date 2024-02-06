namespace ROA.Utilities.Models;

public class PagedCursorList<T> 
{
    protected PagedCursorList()
    {
    }

    public PagedCursorList(IEnumerable<T> items, int totalItems, string lastId)
    {
        Items = new List<T>(items);
        TotalItems = totalItems;
        LastId = lastId;
    }

    public List<T> Items { get; set; }

    public int TotalItems { get; set; }

    public string LastId { get; set; }
}