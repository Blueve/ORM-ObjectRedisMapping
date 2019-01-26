namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Reflection;

    /// <summary>
    /// The type metadata.
    /// </summary>
    internal class TypeMetadata
    {
        /// <summary>
        /// Initialze an instance of <see cref="TypeMetadata"/>.
        /// </summary>
        /// <param name="type">The type.</param>
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
        /// The type name.
        /// </summary>
        public string Name { get; set; }
    }
}
