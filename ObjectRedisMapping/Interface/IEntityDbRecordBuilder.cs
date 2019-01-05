namespace Blueve.ObjectRedisMapping
{
    using System.Collections.Generic;

    /// <summary>
    /// The interface
    /// </summary>
    internal interface IEntityDbRecordBuilder
    {
        /// <summary>
        /// Generate database records from the entity.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The records.</returns>
        IEnumerable<DbRecord> Generate<T>(T entity);
    }
}
