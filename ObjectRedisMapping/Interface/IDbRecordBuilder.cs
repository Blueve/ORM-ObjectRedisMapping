namespace Blueve.ObjectRedisMapping
{
    using System.Collections.Generic;

    /// <summary>
    /// The interface
    /// </summary>
    internal interface IDbRecordBuilder
    {
        /// <summary>
        /// Generate database records from the object.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="prefix">The prefix, default is an empty string.</param>
        /// <returns>The records.</returns>
        IEnumerable<DbRecord> Generate<T>(T obj, string prefix = "");
    }
}
