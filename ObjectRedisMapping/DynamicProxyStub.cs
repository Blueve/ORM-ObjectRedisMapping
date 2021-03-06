﻿namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using StackExchange.Redis;

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
        /// The database record builder.
        /// </summary>
        private readonly IDbRecordBuilder dbRecordBuilder;

        /// <summary>
        /// The database client.
        /// </summary>
        internal readonly IDatabase dbClient;

        /// <summary>
        /// Initialzie an instance of <see cref="DynamicProxyStub"/>.
        /// </summary>
        /// <param name="typeRepo">The type repository.</param>
        /// <param name="dbClient">The database client.</param>
        /// <param name="dbRecordBuilder">The database record builder.</param>
        internal DynamicProxyStub(
            TypeRepository typeRepo,
            IDatabase dbClient,
            IDbRecordBuilder dbRecordBuilder)
        {
            this.typeRepo = typeRepo;
            this.dbClient = dbClient;
            this.dbRecordBuilder = dbRecordBuilder;
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
            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this, this.dbClient);
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
            var entityKey = EntityKeyGenerator.GetEntityKey(typeMetadata, value);

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

            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this, this.dbClient);
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

            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this, this.dbClient, true);
            return proxyGenerator.GenerateForObject<T>(dbKey);
        }

        /// <summary>
        /// The setter for object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The object or proxy of object.</param>
        public void ObjectSetter<T>(string dbKey, T value)
        {
            var records = this.dbRecordBuilder.Generate<T>(value, dbKey);
            records.AddOrUpdate(this.dbClient);
        }

        /// <summary>
        /// The getter for list type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The proxy of list.</returns>
        public IList<T> ListGetter<T>(string dbKey)
        {
            if (!this.dbClient.KeyExists(dbKey))
            {
                return default;
            }

            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this, this.dbClient);
            return proxyGenerator.GenerateForList<T>(dbKey);
        }

        /// <summary>
        /// The readonly getter for list type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The proxy of list which all property are readonly.</returns>
        public IList<T> ReadOnlyListGetter<T>(string dbKey)
        {
            if (!this.dbClient.KeyExists(dbKey))
            {
                return default;
            }

            var proxyGenerator = new DynamicProxyGenerator(this.typeRepo, this, this.dbClient, true);
            return proxyGenerator.GenerateForList<T>(dbKey);
        }

        /// <summary>
        /// The setter for list type.
        /// </summary>
        /// <typeparam name="T">The list type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <param name="value">The list or proxy of list.</param>
        public void ListSetter<T, TElem>(string dbKey, T value)
            where T : IList<TElem>
        {
            var list = value as IList<TElem>;
            var records = this.dbRecordBuilder.Generate<IList<TElem>>(list, dbKey);
            records.AddOrUpdate(this.dbClient);
        }
    }
}
