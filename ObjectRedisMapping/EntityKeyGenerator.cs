namespace Blueve.ObjectRedisMapping
{
    using System;

    /// <summary>
    /// The entity database key generator.
    /// </summary>
    internal class EntityKeyGenerator
    {
        /// <summary>
        /// The entity value serializer.
        /// </summary>
        private readonly EntityKeyValueFormatter entityKeyValueFormatter;

        /// <summary>
        /// Initialize an instance of <see cref="EntityKeyGenerator"/>.
        /// </summary>
        /// <param name="entityKeyValueFormatter"><see cref="EntityKeyValueFormatter"/>.</param>
        public EntityKeyGenerator(EntityKeyValueFormatter entityKeyValueFormatter)
        {
            this.entityKeyValueFormatter = entityKeyValueFormatter;
        }

        /// <summary>
        /// Gets the database key of an entity.
        /// </summary>
        /// <param name="typeMetadata">The type metadata of entity.</param>
        /// <param name="entityKey">The entity key.</param>
        /// <returns>The databse key.</returns>
        public string GetDbKey(TypeMetadata typeMetadata, string entityKey)
        {
            return $"{typeMetadata.Name}{this.entityKeyValueFormatter.Format(entityKey)}";
        }

        /// <summary>
        /// Gets the database key of an entity.
        /// </summary>
        /// <param name="typeMetadata">The type metadata of entity.</param>
        /// <param name="entity">The value of entity.</param>
        /// <returns>The databse key.</returns>
        public string GetDbKey(TypeMetadata typeMetadata, object entity)
        {
            return this.GetDbKey(typeMetadata, this.GetEntityKey(typeMetadata, entity));
        }

        /// <summary>
        /// Gets the entity key.
        /// </summary>
        /// <param name="typeMetadata">The type metadata of entity.</param>
        /// <param name="entity">The value of entity.</param>
        /// <returns>The entity key.</returns>
        public string GetEntityKey(TypeMetadata typeMetadata, object entity)
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
