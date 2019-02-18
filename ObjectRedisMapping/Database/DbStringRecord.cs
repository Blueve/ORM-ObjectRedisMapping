namespace Blueve.ObjectRedisMapping
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using StackExchange.Redis;

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
        public async Task AddOrUpdate(IDatabaseAsync db)
        {
            await db.StringSetAsync(this.Key, this.Value);
        }
    }
}
