namespace ROA.Model.Contract;

public interface IEntity: IPrimaryKeyModel
{
    Guid ETag { get; set; }
}