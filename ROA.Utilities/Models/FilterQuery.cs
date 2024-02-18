using ROA.Utilities.Enums;

namespace ROA.Utilities.Models;

public class FilterQuery: IFilterContainer
{
    public List<Filter> Filters { get; set; } = new List<Filter>();

    public string Search { get; set; }
    
    public string OrderBy { get; set; }
    
    public OrderDirectionType OrderDirection { get; set; }
}