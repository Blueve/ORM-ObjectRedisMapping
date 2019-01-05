namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Text;

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
        /// <param name="typeMetdata">The type metadata of entity.</param>
        /// <param name="entityKeyValue">The value of entity key.</param>
        /// <returns>The databse key.</returns>
        public string GetDbKey(TypeMetadata typeMetdata, string entityKeyValue)
        {
            return $"{typeMetdata.Name}{this.entityKeyValueFormatter.Format(entityKeyValue)}";
        }

        /// <summary>
        /// Gets the database key of an entity.
        /// </summary>
        /// <param name="typeMetdata">The type metadata of entity.</param>
        /// <param name="entity">The value of entity.</param>
        /// <returns>The databse key.</returns>
        public string GetDbKey(TypeMetadata typeMetdata, object entity)
        {
            var key = typeMetdata.KeyProperty.GetValue(entity);

            if (typeMetdata.KeyAttribute.UseInterface)
            {
                // Use GetKey() to generate the key of entity if user indicate that property implemented IEntityKey.
                if (key is IEntityKey entityKey)
                {
                    return this.GetDbKey(typeMetdata, entityKey.GetKey());
                }

                throw new InvalidOperationException(
                    $"Type {typeMetdata.Name} dosen't implement interface IEntityKey.");
            }
            else
            {
                // Use ToString() to generate the key of entity by default.
                return this.GetDbKey(typeMetdata, key.ToString());
            }
        }
    }
}
