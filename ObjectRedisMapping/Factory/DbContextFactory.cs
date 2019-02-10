namespace Blueve.ObjectRedisMapping
{
    using StackExchange.Redis;

    /// <summary>
    /// The factor of <see cref="DbContext"/>.
    /// </summary>
    public class DbContextFactory
    {
        /// <summary>
        /// The configuration of database context factory.
        /// </summary>
        private readonly Config config;

        /// <summary>
        /// Initialize an instance of <see cref="DbContextFactory"/>.
        /// </summary>
        public DbContextFactory()
        {
            // Use default configurations.
            this.config = new Config();
        }

        /// <summary>
        /// Initialize an instance of <see cref="DbContextFactory"/>.
        /// </summary>
        /// <param name="config">The configuration of database context factory.</param>
        public DbContextFactory(Config config)
        {
            this.config = config;
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
        public IDbContext Create(IDatabaseClient dbClient)
        {
            var typeRepo = this.CreateTypeRepo();

            var entityKeyGenerator = new EntityKeyGenerator();
            var dbRecordBuilder = new DbRecordBuilder(typeRepo, entityKeyGenerator);

            var proxyStub = new DynamicProxyStub(typeRepo, dbClient, dbRecordBuilder, entityKeyGenerator);
            var proxyGenerator = new DynamicProxyGenerator(typeRepo, entityKeyGenerator, proxyStub, dbClient);

            return new DbContext(typeRepo, dbRecordBuilder, proxyGenerator, dbClient);
        }

        internal virtual TypeRepository CreateTypeRepo()
        {
            var typeMetadataGenerator = new TypeMetadataGenerator(this.config.UseFullTypeName);
            return new TypeRepository(typeMetadataGenerator);
        }
    }
}
