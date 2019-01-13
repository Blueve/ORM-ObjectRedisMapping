namespace Blueve.ObjectRedisMapping
{
    /// <summary>
    /// The interface of entity proxy generator.
    /// </summary>
    internal interface IProxyGenerator
    {
        /// <summary>
        /// Generate a proxy for the given entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entityKey">The entity key.</param>
        /// <returns>The proxy.</returns>
        T GenerateForEntity<T>(string entityKey) where T : class;

        /// <summary>
        /// Generate a proxy for the given object type.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="entityKey">The prefix of database key.</param>
        /// <returns>The proxy.</returns>
        T GenerateForObject<T>(string dbPrefix) where T : class;
    }
}
