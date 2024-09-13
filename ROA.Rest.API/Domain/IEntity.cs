namespace ROA.Rest.API.Domain;

public interface IEntity: IPrimaryKeyModel
{
    Guid ETag { get; set; }
}