namespace Blueve.ObjectRedisMapping
{
    /// <summary>
    /// The databse record.
    /// </summary>
    internal readonly struct DbRecord
    {
        /// <summary>
        /// The database key.
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// The database value.
        /// </summary>
        public readonly DbValue Value;

        /// <summary>
        /// Initialize an instance of <see cref="DbRecord"/>.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <param name="value">The database value.</param>
        public DbRecord(string key, DbValue value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
}
