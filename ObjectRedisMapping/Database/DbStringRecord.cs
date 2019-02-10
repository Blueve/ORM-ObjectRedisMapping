using System.Diagnostics;

namespace Blueve.ObjectRedisMapping
{
    /// <summary>
    /// The database string record.
    /// </summary>
    [DebuggerDisplay("{Key} - {Value}")]
    internal readonly struct DbStringRecord : IDbOperation
    {
        /// <summary>
        /// The database key.
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// The database value.
        /// </summary>
        public readonly string Value;

        /// <summary>
        /// Initialize an instance of <see cref="DbStringRecord"/>.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <param name="value">The database value.</param>
        public DbStringRecord(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <inheritdoc/>
        public void AddOrUpdate(IDatabaseClient db)
        {
            db.StringSet(this.Key, this.Value);
        }
    }
}
