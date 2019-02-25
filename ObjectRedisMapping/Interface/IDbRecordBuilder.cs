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
        IEnumerable<IDbOperation> Generate<T>(T obj, string prefix = "");

        /// <summary>
        /// Generate a single string database records for the given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="prefix">The prefix.</param>
        /// <param name="obj">The object.</param>
        /// <returns>The records.</returns>
        IEnumerable<IDbOperation> GenerateStringRecord<T>(string prefix, T obj);

        /// <summary>
        /// Generate database records for the given object.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="obj">The object.</param>
        /// <param name="typeMetadata">The type metadata of the object.</param>
        /// <param name="generateRefForProxy">Generate reference for entity if this value set to true.</param>
        /// <returns>The records.</returns>
        IEnumerable<IDbOperation> GenerateObjectRecord(string prefix, object obj, TypeMetadata typeMetadata, bool generateRefForProxy = true);
    }
}
