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
    internal class EntityMetadata : ObjectMetadata
    {
        /// <summary>
        /// Initialize an instacne of <see cref="ObjectMetadata"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The type name.</param>
        /// <param name="properties">The properties of entity.</param>
        /// <param name="keyProperty">The entity key property.</param>
        /// <param name="keyAttr">The key attribute of entity key property.</param>
        public EntityMetadata(
            Type type,
            string name,
            IEnumerable<PropertyInfo> properties,
            PropertyInfo keyProperty,
            EntityKeyAttribute keyAttr)
            : base(type, name, properties.Where(prop => prop != keyProperty))
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

        /// <inheritdoc/>
        protected override string StubGetterMethodName => string.Concat("EntityGetter");

        /// <inheritdoc/>
        protected override string StubSetterMethodName => string.Concat("EntitySetter");

        /// <inheritdoc/>
        public override void RegisterSubType(TypeRepository typeRepo)
        {
            typeRepo.RegisterKeyProperty(this.KeyProperty.PropertyType);
            base.RegisterSubType(typeRepo);
        }

        /// <inheritdoc/>
        public override void RegisterAsKeyType(TypeRepository typeRepo)
        {
            throw new ArgumentException("Key property cannot be an entity type or include an entity type.");
        }
    }
}
