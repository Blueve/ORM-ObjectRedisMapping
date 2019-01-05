﻿namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The metadata generator of type.
    /// </summary>
    internal class TypeMetadataGenerator
    {
        /// <summary>
        /// Generate the metadata for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The metadata of the type.</returns>
        public TypeMetadata Generate(Type type)
        {
            if (type.IsPrimitive)
            {
                // The type is primitive type.
                return new TypeMetadata(type)
                {
                    ValueType = ObjectValueType.Primitive,
                    Name = type.Name
                };
            }

            if (type == typeof(string))
            {
                // The type is string.
                return new TypeMetadata(type)
                {
                    ValueType = ObjectValueType.String,
                    Name = type.Name
                };
            }

            // TODO: Deal the containers and array type.
            ////

            // Only count the virtual property.
            var properties = type
                .GetProperties()
                .Where(prop => prop.GetMethod.IsVirtual && prop.SetMethod.IsVirtual)
                .ToArray();
            if (TryExtractKeyProperty(properties, out var key))
            {
                // The type is an entity because it has a key.
                var (keyAttr, keyProp) = key;
                return new TypeMetadata(type)
                {
                    ValueType = ObjectValueType.Entity,
                    Properties = properties,
                    KeyProperty = keyProp,
                    KeyAttribute = keyAttr,
                    Name = type.FullName // TODO: We can let user to decide that use short name or full name.
                };
            }
            else
            {
                // The type is a user defined type but not declared as an entity.
                return new TypeMetadata(type)
                {
                    ValueType = ObjectValueType.Object,
                    Properties = properties,
                    Name = type.FullName
                };
            }

        }

        /// <summary>
        /// Try extract the key property from all available properties.
        /// </summary>
        /// <param name="properties">The properties list.</param>
        /// <param name="key">The key attribute and key property.</param>
        /// <returns>True if extract successfully.</returns>
        private static bool TryExtractKeyProperty(PropertyInfo[] properties, out (EntityKeyAttribute KeyAttr, PropertyInfo KeyProp) key)
        {
            foreach (var prop in properties)
            {
                var keyAttr = prop.GetCustomAttribute<EntityKeyAttribute>();
                if (keyAttr != null)
                {
                    key = (keyAttr, prop);
                    return true;
                }
            }

            key = (null, null);
            return false;
        }
    }
}
