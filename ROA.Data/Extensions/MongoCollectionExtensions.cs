using MongoDB.Driver;
using ROA.Data.Contract;
using ROA.Model.Contract;
using ROA.Utilities.Enums;
using ROA.Utilities.Models;

namespace ROA.Data.Extensions;

public static class MongoCollectionExtensions
{
    public static IMongoCollection<TDocument> GetCollection<TDocument>(this IMongoDatabase database)
    {
        return database.GetCollection<TDocument>(typeof(TDocument).Name);
    }

    public static IMongoCollection<TDocument> GetCollection<TDocument, TRoot>(this IMongoDatabase database)
    {
        return database.GetCollection<TDocument>(typeof(TRoot).Name);
    }

    public static async Task<(int totalPages, IEnumerable<TDocument> data)> AggregateByPage<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filterDefinition,
        SortDefinition<TDocument> sortDefinition,
        int page,
        int pageSize,
        IDataContext context)
        where TDocument: IEntity
    {
        var countFacet = AggregateFacet.Create("count",
            PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<TDocument>()
            }));

        var dataFacet = AggregateFacet.Create("data",
            PipelineDefinition<TDocument, TDocument>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Sort(sortDefinition),
                PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * pageSize),
                PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize),
            }));

        var aggregation = await collection.Aggregate<TDocument>()
            .Match(filterDefinition)
            .Facet(countFacet, dataFacet)
            .ToListAsync();

        var count = aggregation.First()
            .Facets.First(x => x.Name == "count")
            .Output<AggregateCountResult>()
            ?.FirstOrDefault()
            ?.Count;

        var totalPages = (int)Math.Ceiling((double)count / pageSize);

        IEnumerable<TDocument> data = aggregation.First()
            .Facets.First(x => x.Name == "data")
            .Output<TDocument>();

        data = data.LoadToContext(context);

        return (totalPages, data);
    }

    public static async Task<PagedList<TDocument>> AggregateByPage<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filterDefinition,
        PageQuery query,
        IDataContext context)
        where TDocument : IEntity
    {
        return await collection.Aggregate()
            .Match(filterDefinition)
            .Match(BuildPageSearchFilterDefinition<TDocument>(query))
            .AggregateByPage(query, context);
    }
        
    public static async Task<PagedList<TDocument>> AggregateByPage<TDocument>(
        this IAggregateFluent<TDocument> aggregate,
        PageQuery query, IDataContext context)
    {
        var pipeline = new List<IPipelineStageDefinition>();

        if (!string.IsNullOrEmpty(query.OrderBy))
        {
            var sort = query.OrderDirection == OrderDirectionType.Desc
                ? Builders<TDocument>.Sort.Descending(query.OrderBy)
                : Builders<TDocument>.Sort.Ascending(query.OrderBy);

            pipeline.Add(PipelineStageDefinitionBuilder.Sort(sort));
        }

        if (query.Size > 0)
        {
            pipeline.Add(PipelineStageDefinitionBuilder.Skip<TDocument>((query.Page - 1) * query.Size));
            pipeline.Add(PipelineStageDefinitionBuilder.Limit<TDocument>(query.Size));
        }

        var (count, data) = await AggregateByPage(aggregate, pipeline, context);
            
        var totalPages = (int)Math.Ceiling((double)count / query.Size);
        return new PagedList<TDocument>(data, (int)count, totalPages, query.Page);
    }

    public static async Task<PagedCursorList<TDocument>> AggregateByPage<TDocument>(
        this IAggregateFluent<TDocument> aggregate,
        PageCursorQuery query, FilterDefinition<TDocument> skipFilter, IDataContext context)
        where TDocument: IPrimaryKeyModel
    {
        return await AggregateByPage(aggregate, query, skipFilter, x => x.Id, context);
    }

    public static async Task<PagedCursorList<TDocument>> AggregateByPage<TDocument>(
        this IAggregateFluent<TDocument> aggregate,
        PageCursorQuery query, FilterDefinition<TDocument> skipFilter, Func<TDocument, string> getLastId,
        IDataContext context)
    {
        var pipeline = new List<IPipelineStageDefinition>();

        if (!string.IsNullOrEmpty(query.OrderBy))
        {
            var sort = query.OrderDirection == OrderDirectionType.Desc
                ? Builders<TDocument>.Sort.Descending(query.OrderBy)
                : Builders<TDocument>.Sort.Ascending(query.OrderBy);

            pipeline.Add(PipelineStageDefinitionBuilder.Sort(sort));
        }
            
        if (skipFilter != null && skipFilter != FilterDefinition<TDocument>.Empty)
        {
            pipeline.Add(PipelineStageDefinitionBuilder.Match(skipFilter));
        }

        if (query.Size > 0)
        {
            pipeline.Add(PipelineStageDefinitionBuilder.Limit<TDocument>(query.Size));
        }

        var (count, data) = await AggregateByPage(aggregate, pipeline, context);

        var lastId = data!= null && data.Any()? getLastId(data.LastOrDefault()): null;  
        return new PagedCursorList<TDocument>(data, (int)count, lastId);
    }

    public static FilterDefinition<TDocument> BuildPageSearchFilterDefinition<TDocument>(ISearchPageQuery query)
    {
        if (query.Filter is null || !query.FilterBy.Any()) return FilterDefinition<TDocument>.Empty;
            
        const string pattern = "/{0}/i";

        var dataFilters = query.Filter.Split(" ");

        var andFilters = new List<FilterDefinition<TDocument>>();

        foreach (var data in dataFilters)
        {
            var orFilters = query.FilterBy
                .Select(field => Builders<TDocument>.Filter.Regex(field, string.Format(pattern, data)))
                .ToList();
                
            andFilters.Add(Builders<TDocument>.Filter.Or(orFilters));
        }

        return Builders<TDocument>.Filter.And(andFilters);
    }


    private static async Task<(long count, IEnumerable<TDocument> data)> AggregateByPage<TDocument>(
        this IAggregateFluent<TDocument> aggregate, IEnumerable<IPipelineStageDefinition> dataPipeline,
        IDataContext context)
    {
        const string countFacetName = "count";
        var countFacet = AggregateFacet.Create(countFacetName,
            PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<TDocument>()
            }));

        const string dataFacetName = "data";
        var dataFacet = AggregateFacet.Create(dataFacetName,
            PipelineDefinition<TDocument, TDocument>.Create(dataPipeline));
            
        var aggregation = await aggregate
            .Facet(countFacet, dataFacet)
            .ToListAsync();

        var count = aggregation.First()
            .Facets.First(x => x.Name == countFacetName)
            .Output<AggregateCountResult>()
            ?.FirstOrDefault()
            ?.Count ?? 0;

        IEnumerable<TDocument> data = aggregation.First()
            .Facets.First(x => x.Name == dataFacetName)
            .Output<TDocument>();

        if (typeof(IEntity).IsAssignableFrom(typeof(TDocument)))
        {
            data = data.Cast<IEntity>().LoadToContext(context).Cast<TDocument>();
        }

        return (count, data);
    }
}