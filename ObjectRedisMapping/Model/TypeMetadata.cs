namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Reflection;

    /// <summary>
    /// The type metadata dictionary.
    /// </summary>
    internal class TypeMetadata
    {
        public TypeMetadata(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// The origin type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The object value type.
        /// </summary>
        public ObjectValueType ValueType { get; set; }

        /// <summary>
        /// The properties which can be mapping to database.
        /// </summary>
        public PropertyInfo[] Properties { get; set; }

        /// <summary>
        /// The property which hold the key of entity.
        /// </summary>
        public PropertyInfo KeyProperty { get; set; }

        /// <summary>
        /// The attribute which is hold by key property.
        /// </summary>
        public EntityKeyAttribute KeyAttribute { get; set; }

        /// <summary>
        /// The type name.
        /// </summary>
        public string Name { get; set; }
    }
}
