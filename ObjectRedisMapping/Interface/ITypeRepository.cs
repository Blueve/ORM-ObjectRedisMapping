namespace Blueve.ObjectRedisMapping
{
    using System;

    /// <summary>
    /// The interface of type repository.
    /// </summary>
    internal interface ITypeRepository
    {
        /// <summary>
        /// Register a type to repository.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        void Register<T>();

        /// <summary>
        /// Register a type to repository.
        /// </summary>
        /// <param name="type">The type.</param>
        void Register(Type type);

        /// <summary>
        /// Gets the type metadata of a given type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The type's type metadata.</returns>
        TypeMetadata Get<T>();

        /// <summary>
        /// Gets the type metadata of a given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type's type metadata.</returns>
        TypeMetadata Get(Type type);

        /// <summary>
        /// Try get the type metadata of a given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeMetadata">The type's type metadata.</param>
        /// <returns>True if get the metadata succeed.</returns>
        bool TryGet(Type type, out TypeMetadata typeMetadata);

        /// <summary>
        /// Gets the type's metadata and create one if the metadata not registered before.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type's type metadata.</returns>
        TypeMetadata GetOrAdd(Type type);
    }
}
