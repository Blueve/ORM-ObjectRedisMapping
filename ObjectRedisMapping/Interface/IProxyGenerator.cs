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
        /// <param name="entityKey">The entity key.</param>
        /// <returns>The proxy.</returns>
        T Generate<T>(string entityKey) where T : class;
    }
}
