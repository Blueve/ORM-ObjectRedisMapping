namespace Blueve.ObjectRedisMapping
{
    using StackExchange.Redis;

    /// <summary>
    /// The factor of <see cref="DbContext"/>.
    /// </summary>
    public class DbContextFactory
    {
        private static readonly DbContextFactory Instance;

        /// <summary>
        /// Get an instance of <see cref="IDbContext"/>.
        /// </summary>
        /// <param name="database">The database client.</param>
        /// <returns>The databse context.</returns>
        public static IDbContext GetDbContext(IDatabaseClient database)
        {
            return Instance.Create(database);
        }

        /// <summary>
        /// Create an instance of <see cref="DbContext"/>.
        /// </summary>
        /// <param name="database">The Redis database provider.</param>
        /// <returns>The database context.</returns>
        public IDbContext Create(IDatabase database)
        {
            var dbClient = new StackExchangeDatabaseAdaptor(database);
            return this.Create(dbClient);
        }

        /// <summary>
        /// Create an instance of <see cref="DbContext"/>.
        /// </summary>
        /// <param name="dbClient">The database client.</param>
        /// <returns>The database context.</returns>
        internal IDbContext Create(IDatabaseClient dbClient)
        {
            var typeRepo = this.CreateTypeRepo();

            var entityKeyGenerator = new EntityKeyGenerator();
            var dbRecordBuilder = new DbRecordBuilder(typeRepo, entityKeyGenerator);

            var dbRecordSubmitter = new DbRecordSubmitter(dbClient);

            var proxyStub = new DynamicProxyStub(typeRepo, dbClient, dbRecordBuilder, entityKeyGenerator, dbRecordSubmitter);
            var proxyGenerator = new DynamicProxyGenerator(typeRepo, entityKeyGenerator, proxyStub, dbClient);

            return new DbContext(typeRepo, dbRecordBuilder, dbRecordSubmitter, proxyGenerator);
        }

        internal virtual TypeRepository CreateTypeRepo()
        {
            var typeMetadataGenerator = new TypeMetadataGenerator();
            return new TypeRepository(typeMetadataGenerator);
        }
    }
}
