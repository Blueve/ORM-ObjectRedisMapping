﻿namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using StackExchange.Redis;

    /// <summary>
    /// The database context.
    /// </summary>
    public class DbContext : IDbContext
    {
        /// <summary>
        /// The type repository.
        /// </summary>
        private readonly TypeRepository typeRepository;

        /// <summary>
        /// The database record builder.
        /// </summary>
        private readonly DbRecordBuilder dbRecordBuilder;

        /// <summary>
        /// The proxy generator.
        /// </summary>
        private readonly DynamicProxyGenerator proxyGenerator;

        /// <summary>
        /// The database client.
        /// </summary>
        private readonly IDatabase dbClient;

        /// <summary>
        /// Initialize an instance of <see cref="DbContext"/>.
        /// </summary>
        /// <param name="typeRepository">The type repository.</param>
        /// <param name="dbRecordBuilder">The database record builder.</param>
        /// <param name="proxyGenerator">The proxy generator.</param>
        /// <param name="dbClient">The database client.</param>
        internal DbContext(
            TypeRepository typeRepository,
            DbRecordBuilder dbRecordBuilder,
            DynamicProxyGenerator proxyGenerator,
            IDatabase dbClient)
        {
            this.typeRepository = typeRepository;
            this.dbRecordBuilder = dbRecordBuilder;
            this.proxyGenerator = proxyGenerator;
            this.dbClient = dbClient;
        }

        /// <inheritdoc/>
        public T Find<T>(string key)
            where T : class
        {
            var typeMetadata = this.typeRepository.GetOrRegister(typeof(T));
            if (typeMetadata.ValueType != ObjectValueType.Entity)
            {
                throw new InvalidOperationException($"The type {typeMetadata.Name} is not an entity type.");
            }

            return this.proxyGenerator.GenerateForEntity<T>(key);
        }

        /// <inheritdoc/>
        public void Save<T>(T entity)
            where T : class
        {
            this.DoOperations<T>(entity, r => r.AddOrUpdate(this.dbClient));
        }

        /// <inheritdoc/>
        public void Remove<T>(string key) where T : class
        {
            var entity = this.Find<T>(key);
            if (entity == null)
            {
                return;
            }

            // TODO: Shell the proxy entity then we can generate full records for it.
            var batch = this.dbClient.CreateBatch();
            this.DoOperations<T>(entity, r => r.Remove(batch));
            batch.Execute();
        }

        /// <inheritdoc/>
        public void Remove<T>(T entity) where T : class
        {
            var batch = this.dbClient.CreateBatch();
            this.DoOperations<T>(entity, r => r.Remove(batch));
            batch.Execute();
        }

        /// <inheritdoc/>
        public T Unpack<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        private void DoOperations<T>(T entity, Func<IDbOperation, Task> action)
        {
            var typeMetadata = this.typeRepository.GetOrRegister(typeof(T));
            if (typeMetadata.ValueType != ObjectValueType.Entity)
            {
                throw new InvalidOperationException($"The type {typeMetadata.Name} is not an entity type.");
            }

            var records = typeMetadata.GenerateDbRecords<T>(this.dbRecordBuilder, string.Empty, entity, false);
            var tasks = new List<Task>();
            foreach (var record in records)
            {
                tasks.Add(action(record));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
