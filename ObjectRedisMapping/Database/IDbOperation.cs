namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
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
        Task AddOrUpdate(IDatabaseAsync db);

        /// <summary>
        /// Remove from database.
        /// </summary>
        /// <param name="db">The database client.</param>
        Task Remove(IDatabaseAsync db);
    }
}
