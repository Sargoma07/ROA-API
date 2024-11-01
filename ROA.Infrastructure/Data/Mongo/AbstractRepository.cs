using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ROA.Infrastructure.Data.Mongo.Exceptions;
using ROA.Infrastructure.Data.Mongo.Extensions;
using ROA.Infrastructure.Domain;

namespace ROA.Infrastructure.Data.Mongo;

public abstract class AbstractRepository<T> where T : IEntity
{
    protected readonly IDataContext Context;

    public AbstractRepository(IDataContext context)
    {
        Context = context;
    }

    protected virtual IMongoQueryable<T> GetQuery()
    {
        return this.GetQuery<T>();
    }

    protected virtual IMongoQueryable<TOutput> GetQuery<TOutput>()
        where TOutput: T
    {
        var mongoQueryable = GetCollection<TOutput>().AsQueryable();
        return mongoQueryable;
    }

    protected IMongoCollection<T> GetCollection()
    {
        return this.GetCollection<T>();
    }

    protected virtual IMongoCollection<TOutput> GetCollection<TOutput>()
        where TOutput: T
    {
        return Context.Database.GetCollection<TOutput,T>();
    }

    protected virtual void AddInsertCommand(T entity)
    {
        var collection = GetCollection();
        Context.AddCommand(() => collection.InsertOneAsync(Context.Session, entity));
    }

    protected void AddUpdateCommand(T entity)
    {
        var collection = GetCollection();
        Context.AddCommand(async () =>
        {
            var oldETag = entity.ETag;
            entity.ETag = Guid.NewGuid();

            var res = await collection.ReplaceOneAsync(Context.Session,
                x => x.Id == entity.Id && x.ETag == oldETag,
                entity);

            if (res.MatchedCount != 1)
            {
                throw new OptimisticConcurrencyException($"Matched {res.MatchedCount} records");
            }
        });
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var data = await GetQuery().ToListAsync();

        return data.LoadToContext(Context);
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        var data = await GetQuery().SingleOrDefaultAsync(x => x.Id == id);
        return data is null ? default : data.LoadToContext(Context);
    }

    public virtual void AddOrUpdate(T entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            AddInsertCommand(entity);
        }
        else
        {
            AddUpdateCommand(entity);
        }

    }

    public virtual void Delete(T entity)
    {
        var collection = GetCollection();

        Context.AddCommand(async () =>
        {
            var oldETag = entity.ETag;
            var res = await collection.DeleteOneAsync(Context.Session,
                x => x.Id == entity.Id && x.ETag == oldETag);

            if (res.DeletedCount != 1)
            {
                throw new OptimisticConcurrencyException($"Matched {res.DeletedCount} records");
            }
        });
    }


}