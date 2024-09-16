using MongoDB.Driver;
using ROA.Infrastructure.Data.Locks;
using ROA.Infrastructure.Data.Mongo.Exceptions;

namespace ROA.Infrastructure.Data.Mongo.Locks;

public class DataLock : IDataLock
{
    private const string SIGNALS_COLLECTION = "dlm_signals";
    private static bool _isInitialized = false;
    private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
    private IMongoDatabase _database;
    private readonly FilterDefinitionBuilder<LockLease> _builder = new FilterDefinitionBuilder<LockLease>();
    private readonly IMongoCollection<LockLease> _locks;
    private readonly IMongoCollection<ReleaseSignal> _signals;
    private readonly string _id;

    public DataLock(IMongoDatabase database, string lockId)
    {
        _database = database;
        _locks = _database.GetCollection<LockLease>("dlm_locks");
        _signals = _database.GetCollection<ReleaseSignal>(SIGNALS_COLLECTION);
        _id = lockId;
    }

    private async Task InializeDbAsync(IMongoDatabase database)
    {
        await _semaphoreSlim.WaitAsync();

        try
        {
            // additional check inside critical section
            if (!_isInitialized)
            {
                var collectionsCursor = await _database.ListCollectionNamesAsync();
                var collections = collectionsCursor.ToList();

                if (!collections.Any(x => x == SIGNALS_COLLECTION))
                {
                    // This collection should be a capped! https://docs.mongodb.com/manual/core/capped-collections/
                    // The size of the capped collection should be enough to put all active locks.
                    // One ReleaseSignal is about 32 bytes, so for 100,000 simultaneously locks,
                    // you need a capped collection size ~3 megabytes

                    var options = new CreateCollectionOptions
                    {
                        Capped = true,
                            
                        MaxSize = 32*100000
                    };

                    await _database.CreateCollectionAsync(SIGNALS_COLLECTION, options);

                    // initialize mongo collecvtion
                    await _signals.InsertOneAsync(new ReleaseSignal { AcquireId = Guid.Empty });
                    _isInitialized = true;
                }
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task<ILockLease> AcquireAsync(TimeSpan? lifetime = null, TimeSpan? timeout = null)
    {
        if (!_isInitialized)
        {
            await InializeDbAsync(_database);
        }

        var lifetimeValue = lifetime ?? TimeSpan.FromSeconds(60);
        var timeoutValue = timeout ?? TimeSpan.FromSeconds(15);

        Guid acquireId = Guid.NewGuid();

        while (await TryUpdate(lifetimeValue, acquireId) == false)
        {
            using (IAsyncCursor<LockLease> cursor = await _locks.FindAsync(_builder.Eq(x => x.Id, _id)))
            {
                LockLease acquire = await cursor.FirstOrDefaultAsync();

                if (acquire != null && await WaitSignal(acquire.AcquireId, timeoutValue) == false)
                {
                    var updateResult = await TryUpdate(lifetimeValue, acquireId);

                    if (!updateResult)
                    {
                        throw new LockLeaseException("Lease timeout expired");
                    }

                    return new AcquireResult(acquireId);
                }
            }
        }

        return new AcquireResult(acquireId);
    }

    private async Task<bool> WaitSignal(Guid acquireId, TimeSpan timeout)
    {
        using (IAsyncCursor<ReleaseSignal> cursor = await _signals.Find(x => x.AcquireId == acquireId,
                   new FindOptions { MaxAwaitTime = timeout, CursorType = CursorType.TailableAwait }).ToCursorAsync())
        {
            DateTime started = DateTime.UtcNow;

            while (await cursor.MoveNextAsync())
            {
                if (cursor.Current.Any())
                {
                    return true;
                }

                if (DateTime.UtcNow - started >= timeout)
                {
                    return false;
                }
            }

            return false;
        }
    }

    private async Task<bool> TryUpdate(TimeSpan lifetime, Guid acquireId)
    {
        try
        {
            var update = new UpdateDefinitionBuilder<LockLease>()
                .Set(x => x.IsAcquired, true)
                .Set(x => x.ExpiresIn, DateTime.UtcNow + lifetime)
                .Set(x => x.AcquireId, acquireId)
                .SetOnInsert(x => x.Id, _id);

            FilterDefinition<LockLease> filter = _builder.And(
                _builder.Eq(x => x.Id, _id),
                _builder.Or(
                    _builder.Eq(x => x.IsAcquired, false),
                    _builder.Lte(x => x.ExpiresIn, DateTime.UtcNow)
                )
            );

            var updateResult = await _locks.UpdateOneAsync(
                filter: filter, // x => x.Id == _id && (!x.Acquired || x.ExpiresIn <= DateTime.UtcNow),
                update: update, options: new UpdateOptions { IsUpsert = true });

            return updateResult.IsAcknowledged;
        }
        catch (MongoWriteException ex) // E11000 
        {
            if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                return false;

            throw;
        }
    }

    /// <summary>
    ///  Releases an exclusive lock for the specified acquire. If lock isn't exist or already released, there will be no exceptions throwed
    /// </summary>
    /// <param name="acquire">IAcquire object returned by AcquireAsync</param>
    /// <returns></returns>
    public async Task ReleaseAsync(ILockLease acquire)
    {
        if (acquire == null) throw new ArgumentNullException(nameof(acquire));
        if (acquire.IsAcquired == false) return;

        var updateResult = await _locks.UpdateOneAsync(
            filter: _builder.And(_builder.Eq(x => x.Id, _id), _builder.Eq(x => x.AcquireId, acquire.AcquireId)), // x => x.Id == _id && x.AcquireId == acquire.AcquireId,
            update: new UpdateDefinitionBuilder<LockLease>().Set(x => x.IsAcquired, false));

        if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
            await _signals.InsertOneAsync(new ReleaseSignal { AcquireId = acquire.AcquireId });
    }
}