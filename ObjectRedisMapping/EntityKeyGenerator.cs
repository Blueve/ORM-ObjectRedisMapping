﻿namespace Blueve.ObjectRedisMapping
{
    using System;

    /// <summary>
    /// The entity database key generator.
    /// </summary>
    internal static class EntityKeyGenerator
    {
        /// <summary>
        /// Gets the database key of an entity.
        /// </summary>
        /// <param name="typeMetadata">The type metadata of entity.</param>
        /// <param name="entityKey">The entity key.</param>
        /// <returns>The database key.</returns>
        public static string GetDbKey(EntityMetadata typeMetadata, string entityKey)
        {
            return string.Concat(typeMetadata.Name, entityKey);
        }

        /// <summary>
        /// Gets the database key of an entity.
        /// </summary>
        /// <param name="typeMetadata">The type metadata of entity.</param>
        /// <param name="entity">The value of entity.</param>
        /// <returns>The database key.</returns>
        public static string GetDbKey(EntityMetadata typeMetadata, object entity)
        {
            return GetDbKey(typeMetadata, GetEntityKey(typeMetadata, entity));
        }

        /// <summary>
        /// Gets the entity key.
        /// </summary>
        /// <param name="typeMetadata">The type metadata of entity.</param>
        /// <param name="entity">The value of entity.</param>
        /// <returns>The entity key.</returns>
        public static string GetEntityKey(EntityMetadata typeMetadata, object entity)
        {
            var key = typeMetadata.KeyProperty.GetValue(entity);

            if (typeMetadata.KeyAttribute.UseInterface)
            {
                // Use GetKey() to generate the key of entity if user indicate that property implemented IEntityKey.
                if (key is IEntityKey entityKey)
                {
                    return entityKey.GetKey();
                }

                throw new InvalidOperationException(
                    $"Type {typeMetadata.Name} dosen't implement interface IEntityKey.");
            }
            else
            {
                // Use ToString() to generate the key of entity by default.
                return key.ToString();
            }
        }
    }
}
