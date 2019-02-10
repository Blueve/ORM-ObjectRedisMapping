namespace Blueve.ObjectRedisMapping
{
    using System;

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
        private readonly IDatabaseClient dbClient;

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
            IDatabaseClient dbClient)
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
            var typeMetadata = this.typeRepository.GetOrRegister(typeof(T));
            if (typeMetadata.ValueType != ObjectValueType.Entity)
            {
                throw new InvalidOperationException($"The type {typeMetadata.Name} is not an entity type.");
            }

            var records = typeMetadata.GenerateDbRecords<T>(this.dbRecordBuilder, string.Empty, entity);
            records.AddOrUpdate(this.dbClient);
        }

        /// <inheritdoc/>
        public void Remove<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Remove<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
