namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// The type metadata for entity.
    /// </summary>
    internal class EntityTypeMetadata : ObjectTypeMetadata
    {
        /// <summary>
        /// Initialize an instacne of <see cref="ObjectTypeMetadata"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="properties">The properties of entity.</param>
        /// <param name="keyProperty">The entity key property.</param>
        /// <param name="keyAttr">The key attribute of entity key property.</param>
        public EntityTypeMetadata(Type type, IEnumerable<PropertyInfo> properties, PropertyInfo keyProperty, EntityKeyAttribute keyAttr)
            : base(type, properties.Where(prop => prop != keyProperty))
        {
            this.KeyProperty = keyProperty;
            this.KeyAttribute = keyAttr;
            this.ValueType = ObjectValueType.Entity;
        }

        /// <summary>
        /// The attribute which is hold by key property.
        /// </summary>
        public EntityKeyAttribute KeyAttribute { get; private set; }

        /// <summary>
        /// The property which hold the key of entity.
        /// </summary>
        public PropertyInfo KeyProperty { get; private set; }
    }
}
