namespace Blueve.ObjectRedisMapping
{
    using System.Collections.Generic;

    /// <summary>
    /// The interface
    /// </summary>
    internal interface IDbRecordBuilder
    {
        /// <summary>
        /// Generate database records from the entity.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="prefix">The prefix, default is an empty string.</param>
        /// <returns>The records.</returns>
        IEnumerable<DbRecord> Generate<T>(T entity, string prefix = "");
    }
}
