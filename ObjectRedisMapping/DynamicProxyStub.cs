namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The stub of dynamic proxy.
    /// </summary>
    public class DynamicProxyStub
    {
        /// <summary>
        /// The type repository.
        /// </summary>
        private readonly TypeRepository typeRepo;

        /// <summary>
        /// The database client.
        /// </summary>
        private readonly IDatabaseClient dbClient;

        /// <summary>
        /// The database record builder.
        /// </summary>
        private readonly IDbRecordBuilder dbRecordBuilder;

        /// <summary>
        /// The entity key generator.
        /// </summary>
        private readonly EntityKeyGenerator entityKeyGenerator;

        /// <summary>
        /// Initialzie an instance of <see cref="DynamicProxyStub"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        /// <param name="dbClient">The database client.</param>
        /// <param name="dbRecordBuilder">The database record builder.</param>
        /// <param name="entityKeyGenerator">The entity key generator.</param>
        internal DynamicProxyStub(
            TypeRepository typeRepo,
            IDatabaseClient dbClient,
            IDbRecordBuilder dbRecordBuilder,
            EntityKeyGenerator entityKeyGenerator)
        {
            this.typeRepo = typeRepo;
            this.dbClient = dbClient;
            this.dbRecordBuilder = dbRecordBuilder;
            this.entityKeyGenerator = entityKeyGenerator;
        }

        /// <summary>
        /// The getter for <see cref="string"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The value.</returns>
        public string StringGetter(string dbKey)
        {
            return this.dbClient.KeyExists(dbKey) ? this.dbClient.StringGet(dbKey) : default;
        }

        /// <summary>
        /// The setter for <see cref="string"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The value.</param>
        public void StringSetter(string dbKey, string value)
        {
            this.dbClient.StringSet(dbKey, value);
        }

        /// <summary>
        /// The getter for <see cref="short"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The value.</returns>
        public short Int16Getter(string dbKey)
        {
            return this.dbClient.KeyExists(dbKey) ? Convert.ToInt16(this.dbClient.StringGet(dbKey)) : default;
        }

        /// <summary>
        /// The setter for <see cref="short"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The value.</param>
        public void Int16Setter(string dbKey, short value)
        {
            this.dbClient.StringSet(dbKey, value.ToString());
        }

        /// <summary>
        /// The getter for <see cref="int"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The value.</returns>
        public int Int32Getter(string dbKey)
        {
            return this.dbClient.KeyExists(dbKey) ? Convert.ToInt32(this.dbClient.StringGet(dbKey)) : default;
        }

        /// <summary>
        /// The setter for <see cref="int"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The value.</param>
        public void Int32Setter(string dbKey, int value)
        {
            this.dbClient.StringSet(dbKey, value.ToString());
        }

        /// <summary>
        /// The getter for <see cref="long"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The value.</returns>
        public long Int64Getter(string dbKey)
        {
            return this.dbClient.KeyExists(dbKey) ? Convert.ToInt64(this.dbClient.StringGet(dbKey)) : default;
        }

        /// <summary>
        /// The setter for <see cref="long"/> type.
        /// </summary>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The value.</param>
        public void Int64Setter(string dbKey, long value)
        {
            this.dbClient.StringSet(dbKey, value.ToString());
        }

        /// <summary>
        /// The getter for entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The proxy of entity.</returns>
        public T EntityGetter<T>(string dbKey)
            where T : class
        {
            if (!this.dbClient.KeyExists(dbKey))
            {
                return default;
            }

            var entityKey = this.dbClient.StringGet(dbKey);
            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this.entityKeyGenerator, this, this.dbClient);
            return proxyGenerator.GenerateForEntity<T>(entityKey);
        }

        /// <summary>
        /// The setter for entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The entity or proxy of entity.</param>
        public void EntitySetter<T>(string dbKey, T value)
            where T : class
        {
            var typeMetadata = this.typeRepo.GetOrRegister(typeof(T)) as EntityMetadata;
            var entityKey = this.entityKeyGenerator.GetEntityKey(typeMetadata, value);

            if (!(value is IProxy))
            {
                // If value is not a proxy, then commit value as an entity and then update the DB reference.
                var records = typeMetadata.GenerateDbRecords<T>(this.dbRecordBuilder, entityKey, value);
                records.AddOrUpdate(this.dbClient);
            }
            
            this.dbClient.StringSet(dbKey, entityKey);
        }

        /// <summary>
        /// The getter for object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The proxy of object.</returns>
        public T ObjectGetter<T>(string dbKey)
            where T : class
        {
            if (!this.dbClient.KeyExists(dbKey))
            {
                return default;
            }

            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this.entityKeyGenerator, this, this.dbClient);
            return proxyGenerator.GenerateForObject<T>(dbKey);
        }

        /// <summary>
        /// The readonly getter for object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The proxy of object which all property are readonly.</returns>
        public T ReadOnlyObjectGetter<T>(string dbKey)
            where T : class
        {
            if (!this.dbClient.KeyExists(dbKey))
            {
                return default;
            }

            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this.entityKeyGenerator, this, this.dbClient, true);
            return proxyGenerator.GenerateForObject<T>(dbKey);
        }

        /// <summary>
        /// The settre for object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The object or proxy of object.</param>
        public void ObjectSetter<T>(string dbKey, T value)
        {
            var records = this.dbRecordBuilder.Generate<T>(value, dbKey);
            records.AddOrUpdate(this.dbClient);
        }

        public IEnumerable<T> EnumerableGetter<T>(string dbKey)
        {
            if (!this.dbClient.KeyExists(dbKey))
            {
                return default;
            }

            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this.entityKeyGenerator, this, this.dbClient);
            return proxyGenerator.GenerateForList<T>(dbKey);
        }
    }
}
