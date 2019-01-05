namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The interface of entity recorder.
    /// </summary>
    internal interface IEntityRecorder
    {
        /// <summary>
        /// Commit an entity to data store.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="entity">The entity.</param>
        void Commit<T>(T entity);
    }
}
