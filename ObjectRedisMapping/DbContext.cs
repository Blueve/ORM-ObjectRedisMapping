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
        /// The database record submitter.
        /// </summary>
        private readonly DbRecordSubmitter dbRecordSubmitter;

        /// <summary>
        /// The proxy generator.
        /// </summary>
        private readonly DynamicProxyGenerator proxyGenerator;

        /// <summary>
        /// Initialize an instance of <see cref="DbContext"/>.
        /// </summary>
        /// <param name="typeRepository">The type repository.</param>
        /// <param name="dbRecordBuilder">The database record builder.</param>
        /// <param name="dbRecordSubmitter">The database record submitter.</param>
        /// <param name="proxyGenerator">The proxy generator.</param>
        internal DbContext(
            TypeRepository typeRepository,
            DbRecordBuilder dbRecordBuilder,
            DbRecordSubmitter dbRecordSubmitter,
            DynamicProxyGenerator proxyGenerator)
        {
            this.typeRepository = typeRepository;
            this.dbRecordBuilder = dbRecordBuilder;
            this.dbRecordSubmitter = dbRecordSubmitter;
            this.proxyGenerator = proxyGenerator;
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
        public void Commit<T>(T entity)
            where T : class
        {
            var typeMetadata = this.typeRepository.GetOrRegister(typeof(T));
            if (typeMetadata.ValueType != ObjectValueType.Entity)
            {
                throw new InvalidOperationException($"The type {typeMetadata.Name} is not an entity type.");
            }

            var records = typeMetadata.GenerateDbRecords<T>(this.dbRecordBuilder, string.Empty, entity);
            this.dbRecordSubmitter.Commit(records);
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
