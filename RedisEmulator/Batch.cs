namespace Blueve.RedisEmulator
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using StackExchange.Redis;

    internal class Batch : IBatch
    {
        private readonly IDatabase db;

        public Batch(RedisDatabase db)
        {
            this.db = db;
        }

        public IConnectionMultiplexer Multiplexer => this.db.Multiplexer;

        public Task<RedisValue> DebugObjectAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.DebugObjectAsync(key, flags);
        }

        public void Execute()
        {
            return;
        }

        public Task<RedisResult> ExecuteAsync(string command, params object[] args)
        {
            return this.db.ExecuteAsync(command, args);
        }

        public Task<RedisResult> ExecuteAsync(string command, ICollection<object> args, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ExecuteAsync(command, args, flags);
        }

        public Task<bool> GeoAddAsync(RedisKey key, double longitude, double latitude, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return this.db.GeoAddAsync(key, longitude, latitude, member, flags);
        }

        public Task<bool> GeoAddAsync(RedisKey key, GeoEntry value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.GeoAddAsync(key, value, flags);
        }

        public Task<long> GeoAddAsync(RedisKey key, GeoEntry[] values, CommandFlags flags = CommandFlags.None)
        {
            return this.db.GeoAddAsync(key, values, flags);
        }

        public Task<double?> GeoDistanceAsync(RedisKey key, RedisValue member1, RedisValue member2, GeoUnit unit = GeoUnit.Meters, CommandFlags flags = CommandFlags.None)
        {
            return this.GeoDistanceAsync(key, member1, member2, unit, flags);
        }

        public Task<string[]> GeoHashAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return this.db.GeoHashAsync(key, members, flags);
        }

        public Task<string> GeoHashAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return this.db.GeoHashAsync(key, member, flags);
        }

        public Task<GeoPosition?[]> GeoPositionAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return this.db.GeoPositionAsync(key, members, flags);
        }

        public Task<GeoPosition?> GeoPositionAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return this.GeoPositionAsync(key, member, flags);
        }

        public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, RedisValue member, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            return this.db.GeoRadiusAsync(key, member, radius, unit, count, order, options, flags);
        }

        public Task<GeoRadiusResult[]> GeoRadiusAsync(RedisKey key, double longitude, double latitude, double radius, GeoUnit unit = GeoUnit.Meters, int count = -1, Order? order = null, GeoRadiusOptions options = GeoRadiusOptions.Default, CommandFlags flags = CommandFlags.None)
        {
            return this.db.GeoRadiusAsync(key, longitude, latitude, radius, unit, count, order, options, flags);
        }

        public Task<bool> GeoRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return this.db.GeoRemoveAsync(key, member, flags);
        }

        public Task<long> HashDecrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashDecrementAsync(key, hashField, value, flags);
        }

        public Task<double> HashDecrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashDecrementAsync(key, hashField, value, flags);
        }

        public Task<bool> HashDeleteAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashDeleteAsync(key, hashField, flags);
        }

        public Task<long> HashDeleteAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashDeleteAsync(key, hashFields, flags);
        }

        public Task<bool> HashExistsAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashExistsAsync(key, hashField, flags);
        }

        public Task<HashEntry[]> HashGetAllAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashGetAllAsync(key, flags);
        }

        public Task<RedisValue> HashGetAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashGetAsync(key, hashField, flags);
        }

        public Task<RedisValue[]> HashGetAsync(RedisKey key, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashGetAsync(key, hashFields, flags);
        }

        public Task<Lease<byte>> HashGetLeaseAsync(RedisKey key, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashGetLeaseAsync(key, hashField, flags);
        }

        public Task<long> HashIncrementAsync(RedisKey key, RedisValue hashField, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashIncrementAsync(key, hashField, value, flags);
        }

        public Task<double> HashIncrementAsync(RedisKey key, RedisValue hashField, double value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashIncrementAsync(key, hashField, value, flags);
        }

        public Task<RedisValue[]> HashKeysAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashKeysAsync(key, flags);
        }

        public Task<long> HashLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashLengthAsync(key, flags);
        }

        public Task HashSetAsync(RedisKey key, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashSetAsync(key, hashFields, flags);
        }

        public Task<bool> HashSetAsync(RedisKey key, RedisValue hashField, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashSetAsync(key, hashField, value, when, flags);
        }

        public Task<RedisValue[]> HashValuesAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HashValuesAsync(key, flags);
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HyperLogLogAddAsync(key, value, flags);
        }

        public Task<bool> HyperLogLogAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HyperLogLogAddAsync(key, values, flags);
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HyperLogLogLengthAsync(key, flags);
        }

        public Task<long> HyperLogLogLengthAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HyperLogLogLengthAsync(keys, flags);
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HyperLogLogMergeAsync(destination, first, second, flags);
        }

        public Task HyperLogLogMergeAsync(RedisKey destination, RedisKey[] sourceKeys, CommandFlags flags = CommandFlags.None)
        {
            return this.db.HyperLogLogMergeAsync(destination, sourceKeys, flags);
        }

        public Task<EndPoint> IdentifyEndpointAsync(RedisKey key = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            return this.db.IdentifyEndpointAsync(key, flags);
        }

        public bool IsConnected(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.IsConnected(key, flags);
        }

        public Task<bool> KeyDeleteAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyDeleteAsync(key, flags);
        }

        public Task<long> KeyDeleteAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyDeleteAsync(keys, flags);
        }

        public Task<byte[]> KeyDumpAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyDumpAsync(key, flags);
        }

        public Task<bool> KeyExistsAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyExistsAsync(key, flags);
        }

        public Task<long> KeyExistsAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyExistsAsync(keys, flags);
        }

        public Task<bool> KeyExpireAsync(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyExpireAsync(key, expiry, flags);
        }

        public Task<bool> KeyExpireAsync(RedisKey key, DateTime? expiry, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyExpireAsync(key, expiry, flags);
        }

        public Task<TimeSpan?> KeyIdleTimeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyIdleTimeAsync(key, flags);
        }

        public Task KeyMigrateAsync(RedisKey key, EndPoint toServer, int toDatabase = 0, int timeoutMilliseconds = 0, MigrateOptions migrateOptions = MigrateOptions.None, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyMigrateAsync(key, toServer, toDatabase, timeoutMilliseconds, migrateOptions, flags);
        }

        public Task<bool> KeyMoveAsync(RedisKey key, int database, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyMoveAsync(key, database, flags);
        }

        public Task<bool> KeyPersistAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyPersistAsync(key, flags);
        }

        public Task<RedisKey> KeyRandomAsync(CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyRandomAsync(flags);
        }

        public Task<bool> KeyRenameAsync(RedisKey key, RedisKey newKey, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyRenameAsync(key, newKey, when, flags);
        }

        public Task KeyRestoreAsync(RedisKey key, byte[] value, TimeSpan? expiry = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyRestoreAsync(key, value, expiry, flags);
        }

        public Task<TimeSpan?> KeyTimeToLiveAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyTimeToLiveAsync(key, flags);
        }

        public Task<RedisType> KeyTypeAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.KeyTypeAsync(key, flags);
        }

        public Task<RedisValue> ListGetByIndexAsync(RedisKey key, long index, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListGetByIndexAsync(key, index, flags);
        }

        public Task<long> ListInsertAfterAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListInsertAfterAsync(key, pivot, value, flags);
        }

        public Task<long> ListInsertBeforeAsync(RedisKey key, RedisValue pivot, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListInsertBeforeAsync(key, pivot, value, flags);
        }

        public Task<RedisValue> ListLeftPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListLeftPopAsync(key, flags);
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListLeftPushAsync(key, value, when, flags);
        }

        public Task<long> ListLeftPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListLeftPushAsync(key, values, flags);
        }

        public Task<long> ListLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListLengthAsync(key, flags);
        }

        public Task<RedisValue[]> ListRangeAsync(RedisKey key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListRangeAsync(key, start, stop, flags);
        }

        public Task<long> ListRemoveAsync(RedisKey key, RedisValue value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListRemoveAsync(key, value, count, flags);
        }

        public Task<RedisValue> ListRightPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListRightPopAsync(key, flags);
        }

        public Task<RedisValue> ListRightPopLeftPushAsync(RedisKey source, RedisKey destination, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListRightPopLeftPushAsync(source, destination, flags);
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListRightPushAsync(key, value, when, flags);
        }

        public Task<long> ListRightPushAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListRightPushAsync(key, values, flags);
        }

        public Task ListSetByIndexAsync(RedisKey key, long index, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListSetByIndexAsync(key, index, value, flags);
        }

        public Task ListTrimAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ListTrimAsync(key, start, stop, flags);
        }

        public Task<bool> LockExtendAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            return this.db.LockExtendAsync(key, value, expiry, flags);
        }

        public Task<RedisValue> LockQueryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.LockQueryAsync(key, flags);
        }

        public Task<bool> LockReleaseAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.LockReleaseAsync(key, value, flags);
        }

        public Task<bool> LockTakeAsync(RedisKey key, RedisValue value, TimeSpan expiry, CommandFlags flags = CommandFlags.None)
        {
            return this.db.LockTakeAsync(key, value, expiry, flags);
        }

        public Task<TimeSpan> PingAsync(CommandFlags flags = CommandFlags.None)
        {
            return this.db.PingAsync(flags);
        }

        public Task<long> PublishAsync(RedisChannel channel, RedisValue message, CommandFlags flags = CommandFlags.None)
        {
            return this.db.PublishAsync(channel, message, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ScriptEvaluateAsync(script, keys, values, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(byte[] hash, RedisKey[] keys = null, RedisValue[] values = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ScriptEvaluateAsync(hash, keys, values, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(LuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ScriptEvaluateAsync(script, parameters, flags);
        }

        public Task<RedisResult> ScriptEvaluateAsync(LoadedLuaScript script, object parameters = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.ScriptEvaluateAsync(script, parameters, flags);
        }

        public Task<bool> SetAddAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetAddAsync(key, value, flags);
        }

        public Task<long> SetAddAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetAddAsync(key, values, flags);
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetCombineAndStoreAsync(operation, destination, first, second, flags);
        }

        public Task<long> SetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetCombineAndStoreAsync(operation, destination, keys, flags);
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey first, RedisKey second, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetCombineAsync(operation, first, second, flags);
        }

        public Task<RedisValue[]> SetCombineAsync(SetOperation operation, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetCombineAsync(operation, keys, flags);
        }

        public Task<bool> SetContainsAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetContainsAsync(key, value, flags);
        }

        public Task<long> SetLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetLengthAsync(key, flags);
        }

        public Task<RedisValue[]> SetMembersAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetMembersAsync(key, flags);
        }

        public Task<bool> SetMoveAsync(RedisKey source, RedisKey destination, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetMoveAsync(source, destination, value, flags);
        }

        public Task<RedisValue> SetPopAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetPopAsync(key, flags);
        }

        public Task<RedisValue[]> SetPopAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetPopAsync(key, count, flags);
        }

        public Task<RedisValue> SetRandomMemberAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetRandomMemberAsync(key, flags);
        }

        public Task<RedisValue[]> SetRandomMembersAsync(RedisKey key, long count, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetRandomMembersAsync(key, count, flags);
        }

        public Task<bool> SetRemoveAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetRemoveAsync(key, value, flags);
        }

        public Task<long> SetRemoveAsync(RedisKey key, RedisValue[] values, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SetRemoveAsync(key, values, flags);
        }

        public Task<long> SortAndStoreAsync(RedisKey destination, RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortAndStoreAsync(destination, key, skip, take, order, sortType, by, get, flags);
        }

        public Task<RedisValue[]> SortAsync(RedisKey key, long skip = 0, long take = -1, Order order = Order.Ascending, SortType sortType = SortType.Numeric, RedisValue by = default(RedisValue), RedisValue[] get = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortAsync(key, skip, take, order, sortType, by, get, flags);
        }

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, CommandFlags flags)
        {
            return this.db.SortedSetAddAsync(key, member, score, flags);
        }

        public Task<bool> SortedSetAddAsync(RedisKey key, RedisValue member, double score, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetAddAsync(key, member, score, when, flags);
        }

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, CommandFlags flags)
        {
            return this.db.SortedSetAddAsync(key, values, flags);
        }

        public Task<long> SortedSetAddAsync(RedisKey key, SortedSetEntry[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetAddAsync(key, values, when, flags);
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey first, RedisKey second, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetCombineAndStoreAsync(operation, destination, first, second, aggregate, flags);
        }

        public Task<long> SortedSetCombineAndStoreAsync(SetOperation operation, RedisKey destination, RedisKey[] keys, double[] weights = null, Aggregate aggregate = Aggregate.Sum, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetCombineAndStoreAsync(operation, destination, keys, weights, aggregate, flags);
        }

        public Task<double> SortedSetDecrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetDecrementAsync(key, member, value, flags);
        }

        public Task<double> SortedSetIncrementAsync(RedisKey key, RedisValue member, double value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetIncrementAsync(key, member, value, flags);
        }

        public Task<long> SortedSetLengthAsync(RedisKey key, double min = double.NegativeInfinity, double max = double.PositiveInfinity, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetLengthAsync(key, min, max, exclude, flags);
        }

        public Task<long> SortedSetLengthByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetLengthByValueAsync(key, min, max, exclude, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByRankAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRangeByRankAsync(key, start, stop, order, flags);
        }

        public Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(RedisKey key, long start = 0, long stop = -1, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRangeByRankWithScoresAsync(key, start, stop, order, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByScoreAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRangeByScoreAsync(key, start, stop, exclude, order, skip, take, flags);
        }

        public Task<SortedSetEntry[]> SortedSetRangeByScoreWithScoresAsync(RedisKey key, double start = double.NegativeInfinity, double stop = double.PositiveInfinity, Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude, long skip, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRangeByValueAsync(key, min, max, exclude, skip, take, flags);
        }

        public Task<RedisValue[]> SortedSetRangeByValueAsync(RedisKey key, RedisValue min = default(RedisValue), RedisValue max = default(RedisValue), Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRangeByValueAsync(key, min, max, exclude, order, skip, take, flags);
        }

        public Task<long?> SortedSetRankAsync(RedisKey key, RedisValue member, Order order = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRankAsync(key, member, order);
        }

        public Task<bool> SortedSetRemoveAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRemoveAsync(key, member, flags);
        }

        public Task<long> SortedSetRemoveAsync(RedisKey key, RedisValue[] members, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRemoveAsync(key, members, flags);
        }

        public Task<long> SortedSetRemoveRangeByRankAsync(RedisKey key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRemoveRangeByRankAsync(key, start, stop, flags);
        }

        public Task<long> SortedSetRemoveRangeByScoreAsync(RedisKey key, double start, double stop, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRemoveRangeByScoreAsync(key, start, stop, exclude, flags);
        }

        public Task<long> SortedSetRemoveRangeByValueAsync(RedisKey key, RedisValue min, RedisValue max, Exclude exclude = Exclude.None, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetRemoveRangeByValueAsync(key, min, max, exclude, flags);
        }

        public Task<double?> SortedSetScoreAsync(RedisKey key, RedisValue member, CommandFlags flags = CommandFlags.None)
        {
            return this.db.SortedSetScoreAsync(key, member, flags);
        }

        public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue messageId, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamAcknowledgeAsync(key, groupName, messageId, flags);
        }

        public Task<long> StreamAcknowledgeAsync(RedisKey key, RedisValue groupName, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamAcknowledgeAsync(key, groupName, messageIds, flags);
        }

        public Task<RedisValue> StreamAddAsync(RedisKey key, RedisValue streamField, RedisValue streamValue, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamAddAsync(key, streamField, streamValue, messageId, maxLength, useApproximateMaxLength, flags);
        }

        public Task<RedisValue> StreamAddAsync(RedisKey key, NameValueEntry[] streamPairs, RedisValue? messageId = null, int? maxLength = null, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamAddAsync(key, streamPairs, messageId, maxLength, useApproximateMaxLength, flags);
        }

        public Task<StreamEntry[]> StreamClaimAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamClaimAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
        }

        public Task<RedisValue[]> StreamClaimIdsOnlyAsync(RedisKey key, RedisValue consumerGroup, RedisValue claimingConsumer, long minIdleTimeInMs, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamClaimIdsOnlyAsync(key, consumerGroup, claimingConsumer, minIdleTimeInMs, messageIds, flags);
        }

        public Task<bool> StreamConsumerGroupSetPositionAsync(RedisKey key, RedisValue groupName, RedisValue position, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamConsumerGroupSetPositionAsync(key, groupName, position, flags);
        }

        public Task<StreamConsumerInfo[]> StreamConsumerInfoAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamConsumerInfoAsync(key, groupName, flags);
        }

        public Task<bool> StreamCreateConsumerGroupAsync(RedisKey key, RedisValue groupName, RedisValue? position = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamCreateConsumerGroupAsync(key, groupName, position, flags);
        }

        public Task<long> StreamDeleteAsync(RedisKey key, RedisValue[] messageIds, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamDeleteAsync(key, messageIds, flags);
        }

        public Task<long> StreamDeleteConsumerAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamDeleteConsumerAsync(key, groupName, consumerName, flags);
        }

        public Task<bool> StreamDeleteConsumerGroupAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamDeleteConsumerGroupAsync(key, groupName, flags);
        }

        public Task<StreamGroupInfo[]> StreamGroupInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamGroupInfoAsync(key, flags);
        }

        public Task<StreamInfo> StreamInfoAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamInfoAsync(key, flags);
        }

        public Task<long> StreamLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamLengthAsync(key, flags);
        }

        public Task<StreamPendingInfo> StreamPendingAsync(RedisKey key, RedisValue groupName, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamPendingAsync(key, groupName, flags);
        }

        public Task<StreamPendingMessageInfo[]> StreamPendingMessagesAsync(RedisKey key, RedisValue groupName, int count, RedisValue consumerName, RedisValue? minId = null, RedisValue? maxId = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamPendingMessagesAsync(key, groupName, count, consumerName, minId, maxId, flags);
        }

        public Task<StreamEntry[]> StreamRangeAsync(RedisKey key, RedisValue? minId = null, RedisValue? maxId = null, int? count = null, Order messageOrder = Order.Ascending, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamRangeAsync(key, minId, maxId, count, messageOrder, flags);
        }

        public Task<StreamEntry[]> StreamReadAsync(RedisKey key, RedisValue position, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamReadAsync(key, position, count, flags);
        }

        public Task<RedisStream[]> StreamReadAsync(StreamPosition[] streamPositions, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamReadAsync(streamPositions, countPerStream, flags);
        }

        public Task<StreamEntry[]> StreamReadGroupAsync(RedisKey key, RedisValue groupName, RedisValue consumerName, RedisValue? position = null, int? count = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamReadGroupAsync(key, groupName, consumerName, position, count, flags);
        }

        public Task<RedisStream[]> StreamReadGroupAsync(StreamPosition[] streamPositions, RedisValue groupName, RedisValue consumerName, int? countPerStream = null, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamReadGroupAsync(streamPositions, groupName, consumerName, countPerStream, flags);
        }

        public Task<long> StreamTrimAsync(RedisKey key, int maxLength, bool useApproximateMaxLength = false, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamTrimAsync(key, maxLength, useApproximateMaxLength, flags);
        }

        public Task<long> StringAppendAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringAppendAsync(key, value, flags);
        }

        public Task<long> StringBitCountAsync(RedisKey key, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringBitCountAsync(key, start, end, flags);
        }

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey first, RedisKey second = default(RedisKey), CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringBitOperationAsync(operation, destination, first, second, flags);
        }

        public Task<long> StringBitOperationAsync(Bitwise operation, RedisKey destination, RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringBitOperationAsync(operation, destination, keys, flags);
        }

        public Task<long> StringBitPositionAsync(RedisKey key, bool bit, long start = 0, long end = -1, CommandFlags flags = CommandFlags.None)
        {
            return this.StringBitPositionAsync(key, bit, start, end, flags);
        }

        public Task<long> StringDecrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return this.StringDecrementAsync(key, value, flags);
        }

        public Task<double> StringDecrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringDecrementAsync(key, value, flags);
        }

        public Task<RedisValue> StringGetAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringGetAsync(key, flags);
        }

        public Task<RedisValue[]> StringGetAsync(RedisKey[] keys, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringGetAsync(keys, flags);
        }

        public Task<bool> StringGetBitAsync(RedisKey key, long offset, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringGetBitAsync(key, offset, flags);
        }

        public Task<Lease<byte>> StringGetLeaseAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringGetLeaseAsync(key, flags);
        }

        public Task<RedisValue> StringGetRangeAsync(RedisKey key, long start, long end, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringGetRangeAsync(key, start, end, flags);
        }

        public Task<RedisValue> StringGetSetAsync(RedisKey key, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringGetSetAsync(key, value, flags);
        }

        public Task<RedisValueWithExpiry> StringGetWithExpiryAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringGetWithExpiryAsync(key, flags);
        }

        public Task<long> StringIncrementAsync(RedisKey key, long value = 1, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringIncrementAsync(key, value, flags);
        }

        public Task<double> StringIncrementAsync(RedisKey key, double value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringIncrementAsync(key, value, flags);
        }

        public Task<long> StringLengthAsync(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StreamLengthAsync(key, flags);
        }

        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringSetAsync(key, value, expiry, when, flags);
        }

        public Task<bool> StringSetAsync(KeyValuePair<RedisKey, RedisValue>[] values, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringSetAsync(values, when, flags);
        }

        public Task<bool> StringSetBitAsync(RedisKey key, long offset, bool bit, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringSetBitAsync(key, offset, bit, flags);
        }

        public Task<RedisValue> StringSetRangeAsync(RedisKey key, long offset, RedisValue value, CommandFlags flags = CommandFlags.None)
        {
            return this.db.StringSetRangeAsync(key, offset, value, flags);
        }

        public bool TryWait(Task task)
        {
            return this.db.TryWait(task);
        }

        public void Wait(Task task)
        {
            this.db.Wait(task);
        }

        public T Wait<T>(Task<T> task)
        {
            return this.db.Wait(task);
        }

        public void WaitAll(params Task[] tasks)
        {
            this.db.WaitAll(tasks);
        }
    }
}
