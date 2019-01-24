namespace Blueve.ObjectRedisMapping
{
    /// <summary>
    /// The interface of database client.
    /// </summary>
    public interface IDatabaseClient
    {
        /// <summary>
        /// Return if key exists.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <returns>True if the key is exists.</returns>
        bool KeyExists(string key);

        /// <summary>
        /// Sets a string to database by a key.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <param name="value">The value.</param>
        void StringSet(string key, string value);

        /// <summary>
        /// Gets a string from database by a key.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <returns>The value.</returns>
        string StringGet(string key);
    }
}
