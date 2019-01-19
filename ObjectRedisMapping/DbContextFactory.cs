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
        /// <param name="database">The database provider.</param>
        /// <returns>The databse context.</returns>
        public IDbContext Create(IDatabase database)
        {
            // Use virtual factory to allow end user to switch functions.
            var typeRepo = this.CreateTypeRepo();

            var entityKeyGenerator = new EntityKeyGenerator();
            var dbRecordBuilder = new DbRecordBuilder(typeRepo, entityKeyGenerator);

            var dbAccessor = new StackExchangeDatabaseAdaptor(database);
            var dbRecordSubmitter = new DbRecordSubmitter(dbAccessor);

            var proxyStub = new DynamicProxyStub(typeRepo, dbAccessor, dbRecordBuilder, entityKeyGenerator, dbRecordSubmitter);
            var proxyGenerator = new DynamicProxyGenerator(typeRepo, entityKeyGenerator, proxyStub);

            return new DbContext(typeRepo, dbRecordBuilder, dbRecordSubmitter, proxyGenerator);
        }

        internal virtual TypeRepository CreateTypeRepo()
        {
            var typeMetadataGenerator = new TypeMetadataGenerator();
            return new TypeRepository(typeMetadataGenerator);
        }
    }
}
