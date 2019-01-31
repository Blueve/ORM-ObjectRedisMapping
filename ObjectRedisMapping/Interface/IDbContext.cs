namespace Blueve.ObjectRedisMapping
{
    /// <summary>
    /// The interface of database context.
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// Find an entity by a given key in database.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The instance of entity.</returns>
        T Find<T>(string key) where T : class;

        /// <summary>
        /// Store an entity to database.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void Save<T>(T entity) where T : class;

        /// <summary>
        /// Remove an entity by a given key in database.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="key">The key.</param>
        void Remove<T>(string key) where T : class;

        /// <summary>
        /// Remove an entity by a given entity.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="entity">The proxy of entity or entity.</param>
        void Remove<T>(T entity) where T : class;
    }
}
