namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using StackExchange.Redis;

    /// <summary>
    /// The StackExchangeDatabaseAdaptor.
    /// </summary>
    internal class StackExchangeDatabaseAdaptor : IDbAccessor
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
        public void Add(string key, string member)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string Get(string key)
        {
            return this.database.StringGet(key);
        }

        /// <inheritdoc/>
        public string PopBack(string key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string PopFront(string key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void PushBack(string key, string member)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void PushFront(string key, string member)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Set(string key, string value)
        {
            this.database.StringSet(key, value);
        }

        /// <inheritdoc/>
        public void Set(string key, IList<string> value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Set(string key, ISet<string> value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Set(string key, IDictionary<int, string> value)
        {
            throw new NotImplementedException();
        }
    }
}
