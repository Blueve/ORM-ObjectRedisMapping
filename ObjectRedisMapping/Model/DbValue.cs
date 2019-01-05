namespace Blueve.ObjectRedisMapping
{
    /// <summary>
    /// The databse value.
    /// </summary>
    internal struct DbValue
    {
        /// <summary>
        /// The type in database.
        /// </summary>
        public DbValueType Type;

        /// <summary>
        /// The actual object of current value.
        /// </summary>
        public object Object;
    }
}
