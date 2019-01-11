namespace Blueve.ObjectRedisMapping
{
    /// <summary>
    /// The databse value.
    /// </summary>
    internal readonly struct DbValue
    {
        /// <summary>
        /// The type in database.
        /// </summary>
        public readonly DbValueType Type;

        /// <summary>
        /// The actual object of current value.
        /// </summary>
        public readonly object Object;

        /// <summary>
        /// Initialize an instance of <see cref="DbValue"/>.
        /// </summary>
        /// <param name="key">The database value type.</param>
        /// <param name="value">The database value in actual.</param>
        public DbValue(DbValueType type, object obj)
        {
            this.Type = type;
            this.Object = obj;
        }
    }
}
