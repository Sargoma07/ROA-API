namespace ROA.Utilities.Models;

public class FilteredCursorPageQuery: PageCursorQuery, IFilterContainer
{
    public List<Filter> Filters { get; set; } = new List<Filter>();
    
    public bool HasFilterByField( string fieldName)
    {
        return Filters.Any(x => string.Equals(x.Field, fieldName, StringComparison.CurrentCultureIgnoreCase));
    }
        
    public void ClearFiltersByField(string fieldName)
    {
        Filters.RemoveAll(x => string.Equals(x.Field, fieldName, StringComparison.CurrentCultureIgnoreCase));
    }
}