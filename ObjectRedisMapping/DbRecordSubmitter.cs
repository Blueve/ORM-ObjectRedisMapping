namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The database record submitter.
    /// </summary>
    internal class DbRecordSubmitter
    {
        /// <summary>
        /// The database client.
        /// </summary>
        private readonly IDatabaseClient dbClient;

        /// <summary>
        /// Initialize an instance of <see cref="DbRecordSubmitter"/>.
        /// </summary>
        /// <param name="dbClient">The database client.</param>
        public DbRecordSubmitter(IDatabaseClient dbClient)
        {
            this.dbClient = dbClient;
        }

        /// <summary>
        /// Commit a database record to database.
        /// </summary>
        /// <param name="record">The database record.</param>
        public void Commit(DbRecord record)
        {
            switch (record.Value.Type)
            {
                case DbValueType.String:
                    this.dbClient.StringSet(record.Key, record.Value.Object as string);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Commit database records to database.
        /// There is no transaction guarantee.
        /// TODO: We can use default interface after upgrade to C# 8.
        /// </summary>
        /// <param name="records"></param>
        public void Commit(IEnumerable<DbRecord> records)
        {
            foreach (var record in records)
            {
                this.Commit(record);
            }
        }
    }
}
