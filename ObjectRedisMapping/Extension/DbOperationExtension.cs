namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using StackExchange.Redis;

    /// <summary>
    /// The extension of database operation.
    /// TODO: We can use default interface implementation for this purpose once C# 8 available.
    /// </summary>
    internal static class DbOperationExtension
    {
        /// <summary>
        /// Add or update all database records to database.
        /// </summary>
        /// <param name="records">The records.</param>
        /// <param name="db">The database client.</param>
        public static void AddOrUpdate(this IEnumerable<IDbOperation> records, IDatabase db)
        {
            var batch = db.CreateBatch();
            foreach (var record in records)
            {
                record.AddOrUpdate(batch);
            }

            batch.Execute();
        }
    }
}
