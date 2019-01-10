namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The entity recorder.
    /// </summary>
    internal class EntityRecorder
    {
        /// <summary>
        /// The database accessor.
        /// </summary>
        private readonly IDbAccessor dbAccessor;

        /// <summary>
        /// The database record builder.
        /// </summary>
        private readonly IDbRecordBuilder dbRecordBuilder;

        /// <summary>
        /// Initialize an instance of <see cref="EntityRecorder"/>.
        /// </summary>
        /// <param name="dbAccessor">The databse accessor.</param>
        /// <param name="dbRecordBuilder">The databse record builder.</param>
        public EntityRecorder(
            IDbAccessor dbAccessor,
            IDbRecordBuilder dbRecordBuilder)
        {
            this.dbAccessor = dbAccessor;
            this.dbRecordBuilder = dbRecordBuilder;
        }

        /// <inheritdoc/>
        public void Commit<T>(T entity)
        {
            foreach (var record in this.dbRecordBuilder.Generate(entity))
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
        }
    }
}
