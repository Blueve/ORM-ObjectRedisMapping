namespace Blueve.ObjectRedisMapping
{
    using System.Diagnostics.CodeAnalysis;
    using StackExchange.Redis;

    /// <summary>
    /// The StackExchangeDatabaseAdaptor.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class StackExchangeDatabaseAdaptor : IDatabaseClient
    {
        /// <summary>
        /// The database interface.
        /// </summary>
        private readonly IDatabase database;

        /// <summary>
        /// Initialize an instance of <see cref="StackExchangeDatabaseAdaptor"/>.
        /// </summary>
        /// <param name="database">The database interface.</param>
        public StackExchangeDatabaseAdaptor(IDatabase database)
        {
            this.database = database;
        }

        /// <inheritdoc/>
        public bool KeyExists(string key)
        {
            return this.database.KeyExists(key);
        }

        /// <inheritdoc/>
        public string StringGet(string key)
        {
            return this.database.StringGet(key);
        }

        /// <inheritdoc/>
        public void StringSet(string key, string value)
        {
            this.database.StringSet(key, value);
        }
    }
}
