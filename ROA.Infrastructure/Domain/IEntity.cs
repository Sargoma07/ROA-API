namespace ROA.Infrastructure.Domain;

public interface IEntity: IPrimaryKeyModel
{
    Guid ETag { get; set; }
}