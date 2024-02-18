namespace ROA.Utilities.Models;

public interface ISearchPageQuery
{
    string Filter { get; set; }
    
    IEnumerable<string> FilterBy { get; set; }
}