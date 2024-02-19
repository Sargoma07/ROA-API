using ROA.Utilities.Enums;

namespace ROA.Utilities.Models;

public class Filter
{
    public string FieldSeparator { get; set; } = "->";
    public string Field { get; set; }
    public FilterConditionType Condition { get; set; }
    public string Value { get; set; }
    public FilterValueType ValueType { get; set; } = FilterValueType.Text;
}


public class Filter<TValue>: Filter
{
    public Filter()
    {
        
    }
    
    public Filter(Filter filter)
    {
        FieldSeparator = filter.FieldSeparator;
        Field = filter.Field;
        Condition = filter.Condition;
        ValueType = filter.ValueType;
    }
    
    public new TValue Value { get; set; }
}
