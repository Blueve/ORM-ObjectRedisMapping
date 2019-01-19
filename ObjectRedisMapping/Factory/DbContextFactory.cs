namespace Blueve.ObjectRedisMapping
{
    using StackExchange.Redis;

    /// <summary>
    /// The factor of <see cref="DbContext"/>.
    /// </summary>
    public class DbContextFactory
    {
        /// <summary>
        /// Create an instance of <see cref="DbContext"/>.
        /// </summary>
        /// <param name="database">The Redis database provider.</param>
        /// <returns>The database context.</returns>
        public IDbContext Create(IDatabase database)
        {
            var dbAccessor = new StackExchangeDatabaseAdaptor(database);
            return this.Create(dbAccessor);
        }

        /// <summary>
        /// Create an instance of <see cref="DbContext"/>.
        /// </summary>
        /// <param name="dbAccessor">The database accessor.</param>
        /// <returns>The database context.</returns>
        internal IDbContext Create(IDbAccessor dbAccessor)
        {
            var typeRepo = this.CreateTypeRepo();

            var entityKeyGenerator = new EntityKeyGenerator();
            var dbRecordBuilder = new DbRecordBuilder(typeRepo, entityKeyGenerator);

            var dbRecordSubmitter = new DbRecordSubmitter(dbAccessor);

            var proxyStub = new DynamicProxyStub(typeRepo, dbAccessor, dbRecordBuilder, entityKeyGenerator, dbRecordSubmitter);
            var proxyGenerator = new DynamicProxyGenerator(typeRepo, entityKeyGenerator, proxyStub, dbAccessor);

            return new DbContext(typeRepo, dbRecordBuilder, dbRecordSubmitter, proxyGenerator);
        }

        internal virtual TypeRepository CreateTypeRepo()
        {
            var typeMetadataGenerator = new TypeMetadataGenerator();
            return new TypeRepository(typeMetadataGenerator);
        }
    }
}
