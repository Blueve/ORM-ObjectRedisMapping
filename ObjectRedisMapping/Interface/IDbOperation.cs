namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using StackExchange.Redis;

    /// <summary>
    /// The interface of database operation.
    /// </summary>
    internal interface IDbOperation
    {
        /// <summary>
        /// Add or update the database.
        /// </summary>
        /// <param name="db">The database client.</param>
        void AddOrUpdate(IDatabase db);
    }
}
