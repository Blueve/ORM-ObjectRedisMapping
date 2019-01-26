namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// The type metadata for object.
    /// </summary>
    internal class ObjectTypeMetadata : TypeMetadata
    {
        /// <summary>
        /// Initialize an instacne of <see cref="ObjectTypeMetadata"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="properties">The peoperties of object.</param>
        public ObjectTypeMetadata(Type type, IEnumerable<PropertyInfo> properties)
            : base(type)
        {
            this.Properties = properties.ToArray();
            this.ValueType = ObjectValueType.Object;
        }

        /// <summary>
        /// The properties which can be mapping to database.
        /// </summary>
        public PropertyInfo[] Properties { get; private set; }
    }
}
