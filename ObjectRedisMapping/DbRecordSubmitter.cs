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
        /// The database accessor.
        /// </summary>
        private readonly IDbAccessor dbAccessor;

        /// <summary>
        /// Initialize an instance of <see cref="DbRecordSubmitter"/>.
        /// </summary>
        /// <param name="dbAccessor">The database accessor.</param>
        public DbRecordSubmitter(IDbAccessor dbAccessor)
        {
            this.dbAccessor = dbAccessor;
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
                    this.dbAccessor.Set(record.Key, record.Value.Object as string);
                    break;

                case DbValueType.List:
                    this.dbAccessor.Set(record.Key, record.Value.Object as IList<string>);
                    break;

                case DbValueType.Set:
                    this.dbAccessor.Set(record.Key, record.Value.Object as ISet<string>);
                    break;

                case DbValueType.SortedSet:
                    this.dbAccessor.Set(record.Key, record.Value.Object as IDictionary<int, string>);
                    break;

                default:
                    throw new NotSupportedException();
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
