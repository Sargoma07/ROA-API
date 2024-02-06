using ROA.Utilities.Enums;

namespace ROA.Utilities.Models;

public class PageCursorQuery : ISearchPageQuery
{
    public string LastId { get; set; }

    public int Size { get; set; } = 20;
        
    public string OrderBy { get; set; }

    public OrderDirectionType OrderDirection { get; set; }
    
    public string Filter { get; set; }
    
    public IEnumerable<string> FilterBy { get; set; } = new List<string>();
}