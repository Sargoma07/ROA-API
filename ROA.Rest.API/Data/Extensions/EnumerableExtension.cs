using ROA.Rest.API.Domain;

namespace ROA.Rest.API.Data.Extensions;

public static class EnumerableExtension
{
    internal static IEnumerable<TEntity> LoadToContext<TEntity>(this IEnumerable<TEntity> entities,
        IDataContext context)
        where TEntity : IEntity
    {
        var result = new List<TEntity>();

        if (context.ChangeTracker.TryGetValue(typeof(TEntity), out var loadedSet))
        {
            var newEntries = new List<TEntity>();

            foreach (var entity in entities)
            {
                if (loadedSet.TryGetValue(entity.Id, out var loadedEntity))
                {
                    result.Add((TEntity)loadedEntity);
                }
                else
                {
                    result.Add(entity);
                    newEntries.Add(entity);
                }
            }

            newEntries.ForEach(x => loadedSet.Add(x.Id, x));
        }
        else
        {
            var enumerable = entities as TEntity[] ?? entities.ToArray();
            context.ChangeTracker[typeof(TEntity)] = enumerable.ToDictionary(x => x.Id, x => (IEntity)x);
            result.AddRange(enumerable);
        }

        return result;
    }

    internal static TEntity LoadToContext<TEntity>(this TEntity source,
        IDataContext context)
        where TEntity : IEntity
    {
        return LoadToContext(new[] { source }, context).Single();
    }
}