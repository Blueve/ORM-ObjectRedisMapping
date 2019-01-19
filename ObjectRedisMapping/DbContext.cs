namespace Blueve.ObjectRedisMapping
{
    using System;

    /// <summary>
    /// The database context.
    /// </summary>
    public class DbContext : IDbContext
    {
        private readonly TypeRepository typeRepository;

        private readonly DbRecordBuilder dbRecordBuilder;

        private readonly DbRecordSubmitter dbRecordSubmitter;

        private readonly DynamicProxyGenerator proxyGenerator;

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

            var records = this.dbRecordBuilder.Generate(entity);
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
