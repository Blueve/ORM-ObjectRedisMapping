namespace Blueve.ObjectRedisMapping
{
    using System;

    /// <summary>
    /// The database context.
    /// </summary>
    public class DbContext : IDbContext
    {
        private readonly TypeRepository typeRepository;

        private readonly IEntityRecorder entityRecorder;

        private readonly IProxyGenerator proxyGenerator;

        internal DbContext(
            TypeRepository typeRepository,
            IEntityRecorder entityRecorder,
            IProxyGenerator proxyGenerator)
        {
            this.typeRepository = typeRepository;
            this.entityRecorder = entityRecorder;
            this.proxyGenerator = proxyGenerator;
        }

        /// <inheritdoc/>
        public T Find<T>(string key)
            where T : class
        {
            var typeMetadata = this.typeRepository.GetOrAdd(typeof(T));
            if (typeMetadata.ValueType != ObjectValueType.Entity)
            {
                throw new InvalidOperationException($"The type {typeMetadata.Name} is not an entity type.");
            }

            return this.proxyGenerator.Generate<T>(key);
        }

        /// <inheritdoc/>
        public void Commit<T>(T entity)
            where T : class
        {
            var typeMetadata = this.typeRepository.GetOrAdd(typeof(T));
            if (typeMetadata.ValueType != ObjectValueType.Entity)
            {
                throw new InvalidOperationException($"The type {typeMetadata.Name} is not an entity type.");
            }

            this.entityRecorder.Commit(entity);
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
