namespace Blueve.ObjectRedisMapping
{
    /// <summary>
    /// The interface of entity proxy generator.
    /// </summary>
    internal interface IProxyGenerator
    {
        /// <summary>
        /// Generate a proxy for the given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="dbKey">The database key.</param>
        /// <returns>The proxy.</returns>
        T Generate<T>(string dbKey) where T : class;
    }
}
